using System.Diagnostics.CodeAnalysis;
using DTO.Person;
using Logging.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace WebApp.Shared;

public partial class FilterLifePoints
{
    private const int DefaultYear = -1;
    private static readonly Guid DefaultCreatorId = Guid.Empty;
    private IJSObjectReference _lifePointDetailModule = null!;
    private IEnumerable<ExistingPerson> _distinctCreators = new List<ExistingPerson>();
    private IEnumerable<int> _distinctYears = new List<int>();
    private bool _showFilter;
    private bool _creatorFilterApplied;
    private bool _yearFilterApplied;
    private int? _selectedYear = DefaultYear;
    private Guid? _selectedCreatorId = DefaultCreatorId;

    /// <inheritdoc />
    [SuppressMessage("Design", "MA0119:JSRuntime must not be used in OnInitialized or OnInitializedAsync", Justification = "It works, so I'm fine")]
    protected override async Task OnInitializedAsync()
    {
        Logger.MethodStarted();

        await base.OnInitializedAsync();

        ReloadDistinctCreators();
        ReloadDistinctYears();
        _lifePointDetailModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./Shared/LifePointDetail.razor.js");

        Logger.MethodFinished();
    }

    private void ReloadDistinctYears(Guid? creatorId = null) => _distinctYears = LifePointService.GetDistinctYears(creatorId);

    private void ReloadDistinctCreators(int? year = null) => _distinctCreators = LifePointService.GetDistinctCreators(year);

    private async Task SelectedCreatorChangedAsync(ChangeEventArgs args)
    {
        if (!Guid.TryParse(args.Value?.ToString(), out var creatorId))
        {
            return;
        }

        if (_selectedCreatorId == creatorId)
        {
            return;
        }

        if (creatorId == DefaultCreatorId)
        {
            _selectedCreatorId = null;
            _creatorFilterApplied = false;
        }
        else
        {
            _selectedCreatorId = creatorId;
            _creatorFilterApplied = true;
        }

        await DrawSubsetOfMarkersAsync();
    }

    private async Task SelectedYearChangedAsync(ChangeEventArgs args)
    {
        if (!int.TryParse(args.Value?.ToString(), out var year))
        {
            return;
        }

        if (_selectedYear == year)
        {
            return;
        }

        if (year == DefaultYear)
        {
            _selectedYear = null;
            _yearFilterApplied = false;
        }
        else
        {
            _selectedYear = year;
            _yearFilterApplied = true;
        }

        await DrawSubsetOfMarkersAsync();
    }

    private async Task DrawSubsetOfMarkersAsync()
    {
        await EnableSpinnerAsync();

        await RemoveAllExistingMarkersAsync();

        var yearToFilter = _selectedYear == DefaultYear ? null : _selectedYear;
        var creatorIdToFilter = _selectedCreatorId == DefaultCreatorId ? null : _selectedCreatorId;

        foreach (var (latitude, longitude, id) in LifePointService.GetAllLocations(yearToFilter, creatorIdToFilter))
        {
            await AddMarkerAsync(id, latitude, longitude);
        }

        await DisableSpinnerAsync();
    }

    private async Task RemoveAllExistingMarkersAsync() => await _lifePointDetailModule.InvokeVoidAsync("reset");

    private async Task AddMarkerAsync(Guid id, double latitude, double longitude)
        => await _lifePointDetailModule.InvokeVoidAsync("createMarkerForExistingLifePoint", id, latitude, longitude);

    private async Task EnableSpinnerAsync() => await _lifePointDetailModule.InvokeVoidAsync("enableSpinner");

    private async Task DisableSpinnerAsync() => await _lifePointDetailModule.InvokeVoidAsync("disableSpinner");

    private async Task OnFilterButtonClickAsync()
    {
        _showFilter = !_showFilter;

        if (!_showFilter && (_creatorFilterApplied || _yearFilterApplied))
        {
            await RemoveAllExistingMarkersAsync();

            foreach (var (latitude, longitude, id) in LifePointService.GetAllLocations())
            {
                await AddMarkerAsync(id, latitude, longitude);
            }

            _selectedYear = DefaultYear;
            _selectedCreatorId = DefaultCreatorId;
            _yearFilterApplied = false;
            _creatorFilterApplied = false;
        }
    }
}