using AppLibertadoresHAS.ViewModels;
using Microsoft.Maui.Maps;

namespace AppLibertadoresHAS.Views.Usuarios;

public partial class LocalizacaoView : ContentPage
{
	LocalizacaoViewModel viewModel;
	public LocalizacaoView()
	{
		InitializeComponent();

		viewModel = new LocalizacaoViewModel();
        viewModel.InicializarMapa();
        viewModel.ChecarLocalizacao();
        viewModel.ExibirUsuariosNoMapa();
        viewModel.LocalizacaoObtida += (loc) =>
			MainThread.BeginInvokeOnMainThread(() =>
				mapa.MoveToRegion(MapSpan.FromCenterAndRadius(loc, Distance.FromKilometers(5))));
		
		viewModel.CarregarMapa();

		BindingContext = viewModel;
	}
}