using System.Reactive.Linq;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol.Core.Types;
using ReactiveUI;

namespace WpfApp2;

public class AppViewModel : ReactiveObject
{
    private string _searchTerm;

    public string SearchTerm
    {
        get => _searchTerm;
        set => this.RaiseAndSetIfChanged(ref _searchTerm, value);
    }

    private readonly ObservableAsPropertyHelper<IEnumerable<NugetDetailsViewModel>> _searchResults;
    public IEnumerable<NugetDetailsViewModel> SearchResults => _searchResults.Value;

    private readonly ObservableAsPropertyHelper<bool> _isAvailable;
    public bool IsAvailable => _isAvailable.Value;

    public AppViewModel()
    {
        _searchResults = this
            .WhenAnyValue(x => x.SearchTerm)
            .Throttle(TimeSpan.FromMilliseconds(700))
            .Select(term => term?.Trim())
            .DistinctUntilChanged()
            .Where(term => !string.IsNullOrWhiteSpace(term))
            .SelectMany(SearchNuGetPackages)
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.SearchResults);

        _searchResults.ThrownExceptions.Subscribe(error =>
        {
            // Handle errors here
            // For example, log them
        });

        _isAvailable = this
            .WhenAnyValue(x => x.SearchResults)
            .Select(searchResults => searchResults != null)
            .ToProperty(this, x => x.IsAvailable);
    }

    private async Task<IEnumerable<NugetDetailsViewModel>> SearchNuGetPackages(
        string term, CancellationToken token)
    {
        var providers = new List<Lazy<INuGetResourceProvider>>();
        providers.AddRange(Repository.Provider.GetCoreV3()); // Add v3 API support
        var packageSource = new PackageSource("https://api.nuget.org/v3/index.json");
        var source = new SourceRepository(packageSource, providers);
        ILogger logger = NullLogger.Instance;

        var filter = new SearchFilter(false);
        var resource = await source.GetResourceAsync<PackageSearchResource>().ConfigureAwait(false);
        var metadata = await resource.SearchAsync(term, filter, 0, 10, logger, token).ConfigureAwait(false);
        return metadata.Select(x => new NugetDetailsViewModel(x));
    }
}