using System.Reactive.Disposables;
using ReactiveUI;

namespace WpfApp2;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        ViewModel = new AppViewModel();

        this.WhenActivated(disposableRegistration =>
        {
            this.OneWayBind(ViewModel,
                    viewModel => viewModel.SearchResults,
                    view => view.SearchResultsListBox.ItemsSource)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    viewModel => viewModel.SearchResults,
                    view => view.SearchResultsListBox.ItemsSource)
                .DisposeWith(disposableRegistration);

            this.Bind(ViewModel,
                    viewModel => viewModel.SearchTerm,
                    view => view.SearchTextBox.Text)
                .DisposeWith(disposableRegistration);
        });
    }
}