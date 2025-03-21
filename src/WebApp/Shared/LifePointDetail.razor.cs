﻿using DTO.LifePoint;
using Logging.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace WebApp.Shared;

public partial class LifePointDetail
{
    private ExistingLifePoint _lifePoint = null!; // is initialized on component construction

    private Uri? _imageUri;

    [Parameter]
    public string Id { get; set; } = null!; // is initialized on component construction

    private protected IJSObjectReference LifePointDetailModule { get; set; } = null!; // is initialized on component construction

    /// <inheritdoc />
    protected override async Task OnInitializedAsync() =>
        await Logger.LogMethodStartAndEndAsync(async () =>
        {
            _lifePoint = await LifePointService.GetLifePointAsync(Guid.Parse(Id));
            if (_lifePoint.ImageId != null)
            {
                _imageUri = ConstructImageUri(_lifePoint.ImageId.Value);
            }

            await LoadLifePointDetailModuleAsync();

            await base.OnInitializedAsync();
        });

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (_imageUri == null && !firstRender)
        {
            await UpdatePopupAsync();
        }
    }

    private Uri ConstructImageUri(Guid imageId) => new(new Uri(Navigator.BaseUri), $"api/images/{_lifePoint.CreatedBy.Id}/{imageId.ToString()}");

    private async void OnDeleteClicked() =>
        await Logger.LogMethodStartAndEndAsync(async () =>
        {
            await LifePointService.DeleteLifePointAsync(Guid.Parse(Id));
            await RemoveMarkerAsync();
        });

    private async Task RemoveMarkerAsync()
        => await LifePointDetailModule.InvokeVoidAsync("removeMarkerOfLifePoint", Id);

    private async Task LoadLifePointDetailModuleAsync() =>
        LifePointDetailModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./Shared/LifePointDetail.razor.js");

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
        if (LifePointDetailModule == null!)
        {
            return;
        }

        await LifePointDetailModule.InvokeVoidAsync("updatePopup", Id);
    }
}