﻿using System;
using System.Threading.Tasks;
using Logging.Extensions;
using Microsoft.JSInterop;

namespace WebApp.Shared;

public partial class Map
{
    private IJSObjectReference _mapModule = null!;
    private IJSObjectReference _newLifePointModule = null!;
    private IJSObjectReference _lifePointDetailModule = null!;
    private IJSObjectReference _leafletMap = null!;
    private DotNetObjectReference<Map>? _objRef;

    [JSInvokable]
    public async Task OpenPopupForNewLifePointAsync(double latitude, double longitude)
    {
        Logger.MethodStarted();
        await _newLifePointModule.InvokeVoidAsync("createPopupForNewLifePoint", _objRef, _leafletMap, latitude, longitude);
        Logger.MethodFinished();
    }

    [JSInvokable]
    public async Task AddMarkerAsync(Guid id, double latitude, double longitude)
        => await _lifePointDetailModule.InvokeVoidAsync("createMarkerForExistingLifePoint", id, latitude, longitude);

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Logger.MethodStarted();
            _objRef = DotNetObjectReference.Create(this);
            await InitializeMapAsync(_objRef);
            await AddMarkersForExistingLocationsAsync();
            Logger.MethodFinished();
        }
    }

    private async Task InitializeMapAsync(DotNetObjectReference<Map> dotNetObjectReference)
    {
        _mapModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./Shared/Map.razor.js");
        _leafletMap = await _mapModule.InvokeAsync<IJSObjectReference>("initializeMap", 51.0405849, 13.7478431, 20, dotNetObjectReference);

        _newLifePointModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./Shared/NewLifePoint.razor.js");
        _lifePointDetailModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./Shared/LifePointDetail.razor.js");

        await _lifePointDetailModule.InvokeVoidAsync("initialize", _leafletMap);
    }

    private async Task AddMarkersForExistingLocationsAsync()
    {
        Logger.MethodStarted();

        // TODO mu88: Rethink whether adding all markers at once provides better performance on redraw
        foreach (var (latitude, longitude, id) in LifePointService.GetAllLocations()) { await AddMarkerAsync(id, latitude, longitude); }

        Logger.MethodFinished();
    }

    private void OnUserDialogClose() => StateHasChanged();
}