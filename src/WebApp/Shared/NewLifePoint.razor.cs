using System;
using System.IO;
using System.Threading.Tasks;
using DTO.LifePoint;
using Logging.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Persistence;
using WebApp.Models;

namespace WebApp.Shared;

public partial class NewLifePoint
{
    internal const long MaxAllowedFileSizeInBytes = MaxAllowedFileSizeInMegaBytes * 1024 * 1024;
    private const long MaxAllowedFileSizeInMegaBytes = 20;
    internal bool ImageTooBig;
    internal bool InputIsNoImage;
    private readonly NewLifePointModel _newLifePoint = new();
    private IJSObjectReference _newLifePointModule = null!; // is initialized on component construction 
    private IBrowserFile? _file;
    private bool _showModalSpinner;

    [Parameter]
    public double Latitude { get; set; }

    [Parameter]
    public double Longitude { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await LoadNewLifePointModuleAsync();

        await base.OnInitializedAsync();

        _newLifePoint.Date = NewLifePointDateService.ProposedCreationDate;
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender) await UpdatePopupAsync();
    }

    private async Task CreateNewLifePointAsync()
    {
        Logger.MethodStarted();

        EnableSpinner();

        ImageToCreate? imageToCreate;
        try
        {
            ImageTooBig = false;
            imageToCreate = _file != null ? new ImageToCreate(_file.OpenReadStream(MaxAllowedFileSizeInBytes)) : null;
        }
        catch (IOException)
        {
            Logger.ImageTooBig();
            DisableSpinner();
            ImageTooBig = true;
            return;
        }

        var lifePointToCreate = new LifePointToCreate(_newLifePoint.Date,
                                                      _newLifePoint.Caption,
                                                      _newLifePoint.Description,
                                                      Latitude,
                                                      Longitude,
                                                      UserService.Id ?? throw new NullReferenceException(Loc["UserHasNotBeenSet"]),
                                                      imageToCreate);

        ExistingLifePoint createdLifePoint;
        try
        {
            createdLifePoint = await LifePointService.CreateLifePointAsync(lifePointToCreate);
            InputIsNoImage = false;
        }
        catch (NoImageException)
        {
            DisableSpinner();
            InputIsNoImage = true;
            return;
        }

        NewLifePointDateService.ProposedCreationDate = _newLifePoint.Date;

        DisableSpinner();

        await RemovePopupAsync();
        await AddMarkerAsync(createdLifePoint);

        Logger.MethodFinished();
    }

    private void EnableSpinner()
    {
        _showModalSpinner = true;
        StateHasChanged();
    }

    private void DisableSpinner()
    {
        _showModalSpinner = false;
        StateHasChanged();
    }

    private async Task RemovePopupAsync()
    {
        Logger.MethodStarted();

        await _newLifePointModule.InvokeVoidAsync("removePopupForNewLifePoint");

        Logger.MethodFinished();
    }

    private async Task AddMarkerAsync(ExistingLifePoint existingLifePoint)
    {
        Logger.MethodStarted();

        await _newLifePointModule.InvokeVoidAsync("addMarkerForCreatedLifePoint", existingLifePoint.Id, existingLifePoint.Latitude, existingLifePoint.Longitude);

        Logger.MethodFinished();
    }

    private async Task LoadNewLifePointModuleAsync() => _newLifePointModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./Shared/NewLifePoint.razor.js");

    private void LoadImage(InputFileChangeEventArgs args) => _file = args.File;

    /// <summary>Updates the underlying Leaflet popup so that size and position are correct and visible.</summary>
    /// <remarks>
    ///     Since the Blazor component is rendered lazily (and therefore after the Leaflet popup has been created),
    ///     the popup has to be updated manually. This will resize it according to the Blazor content and
    ///     also pan the map so that it becomes visible.
    ///     If an image is present, this must happen after the image is loaded - otherwise the calculated size of
    ///     the popup is wrong and would therefore be displayed at the wrong map position.
    /// </remarks>
    private async Task UpdatePopupAsync()
    {
        if (_newLifePointModule == null!) return;

        await _newLifePointModule.InvokeVoidAsync("updatePopup");
    }
}