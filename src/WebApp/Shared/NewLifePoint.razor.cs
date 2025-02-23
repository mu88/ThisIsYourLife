using System.Diagnostics.CodeAnalysis;
using DTO.LifePoint;
using Logging.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Persistence;
using WebApp.Models;

namespace WebApp.Shared;

[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Testing")]
public partial class NewLifePoint
{
    internal const long MaxAllowedFileSizeInBytes = MaxAllowedFileSizeInMegaBytes * 1024 * 1024;
    internal bool ImageTooBig;
    internal bool InputIsNoImage;
    private const long MaxAllowedFileSizeInMegaBytes = 20;
    private readonly NewLifePointModel _newLifePoint = new();
    private IBrowserFile? _file;
    private bool _showModalSpinner;

    [Parameter]
    public double Latitude { get; set; }

    [Parameter]
    public double Longitude { get; set; }

    private protected IJSObjectReference NewLifePointModule { get; set; } = null!; // is initialized on component construction

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

        if (!firstRender)
        {
            await UpdatePopupAsync();
        }
    }

    [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP004:Don\'t ignore created IDisposable", Justification = "Okay here due to different lifetime")]
    private async Task CreateNewLifePointAsync()
    {
        await Logger.LogMethodStartAndEndAsync(async () =>
        {
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
                UserService.Id ?? throw new ArgumentNullException(Loc["UserHasNotBeenSet"]),
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
        });
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

    private async Task RemovePopupAsync() =>
        await Logger.LogMethodStartAndEndAsync(async () => await NewLifePointModule.InvokeVoidAsync("removePopupForNewLifePoint"));

    private async Task AddMarkerAsync(ExistingLifePoint existingLifePoint) => await Logger.LogMethodStartAndEndAsync(async () =>
                                                                                  await NewLifePointModule.InvokeVoidAsync("addMarkerForCreatedLifePoint",
                                                                                      existingLifePoint.Id,
                                                                                      existingLifePoint.Latitude,
                                                                                      existingLifePoint.Longitude));

    private async Task LoadNewLifePointModuleAsync() => NewLifePointModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./Shared/NewLifePoint.razor.js");

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
        if (NewLifePointModule == null!)
        {
            return;
        }

        await NewLifePointModule.InvokeVoidAsync("updatePopup");
    }
}