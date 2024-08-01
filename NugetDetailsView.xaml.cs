using System.Reactive.Disposables;
using System.Windows.Media.Imaging;
using ReactiveUI;

namespace WpfApp2;

public partial class NugetDetailsView
{
    public NugetDetailsView()
    {
        InitializeComponent();
        this.WhenActivated(disposableRegistration =>
        {
            this.OneWayBind(ViewModel,
                    viewModel => viewModel.IconUrl,
                    view => view.iconImage.Source,
                    url => url == null ? null : new BitmapImage(url))
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    viewModel => viewModel.Title,
                    view => view.titleRun.Text)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    viewModel => viewModel.Description,
                    view => view.descriptionRun.Text)
                .DisposeWith(disposableRegistration);

            this.BindCommand(ViewModel,
                    viewModel => viewModel.OpenPage,
                    view => view.openButton)
                .DisposeWith(disposableRegistration);
        });
    }
}