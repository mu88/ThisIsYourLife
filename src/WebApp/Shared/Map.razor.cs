﻿using System.Diagnostics.CodeAnalysis;
using Logging.Extensions;
using Microsoft.JSInterop;

namespace WebApp.Shared;

public sealed partial class Map : IDisposable
{
    private IJSObjectReference _mapModule = null!;
    private IJSObjectReference _newLifePointModule = null!;
    private IJSObjectReference _lifePointDetailModule = null!;
    private IJSObjectReference _leafletMap = null!;
    private DotNetObjectReference<Map>? _objRef;
    private bool _disposed;

    [JSInvokable]
    public async Task OpenPopupForNewLifePointAsync(double latitude, double longitude) =>
        await Logger.LogMethodStartAndEndAsync(async () =>
            await _newLifePointModule.InvokeVoidAsync("createPopupForNewLifePoint", _objRef, _leafletMap, latitude, longitude));

    [JSInvokable]
    public async Task AddMarkerAsync(Guid id, double latitude, double longitude)
        => await _lifePointDetailModule.InvokeVoidAsync("createMarkerForExistingLifePoint", id, latitude, longitude);

    [ExcludeFromCodeCoverage(Justification = "Testing the disposal offers no real value")]
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _objRef?.Dispose();

        _disposed = true;
    }

    [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP003:Dispose previous before re-assigning", Justification = "Done in separate method")]
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Logger.LogMethodStartAndEndAsync(async () =>
            {
                _objRef = RecreateReference();
                await InitializeMapAsync(_objRef);
                await AddMarkersForExistingLocationsAsync();
            });
        }
    }

    [ExcludeFromCodeCoverage]
    private DotNetObjectReference<Map> RecreateReference()
    {
        _objRef?.Dispose();
        return DotNetObjectReference.Create(this);
    }

    private async Task InitializeMapAsync(DotNetObjectReference<Map> dotNetObjectReference)
    {
        _mapModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./Shared/Map.razor.js");
        _leafletMap = await _mapModule.InvokeAsync<IJSObjectReference>("initializeMap", 51.0405849, 13.7478431, 20, dotNetObjectReference);

        _newLifePointModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./Shared/NewLifePoint.razor.js");
        _lifePointDetailModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./Shared/LifePointDetail.razor.js");

        await _lifePointDetailModule.InvokeVoidAsync("initialize", _leafletMap);
    }

    private async Task AddMarkersForExistingLocationsAsync() =>
        await Logger.LogMethodStartAndEndAsync(async () =>
        {
            await EnableSpinnerAsync();

            foreach (var (latitude, longitude, id) in LifePointService.GetAllLocations())
            {
                await AddMarkerAsync(id, latitude, longitude);
            }

            await DisableSpinnerAsync();
        });

    private async Task EnableSpinnerAsync() => await _lifePointDetailModule.InvokeVoidAsync("enableSpinner");

    private async Task DisableSpinnerAsync() => await _lifePointDetailModule.InvokeVoidAsync("disableSpinner");

    [ExcludeFromCodeCoverage]
    private void OnUserDialogClose() => StateHasChanged();
}