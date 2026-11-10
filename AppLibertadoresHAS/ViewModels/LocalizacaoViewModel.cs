using AppLibertadoresHAS.Models;
using AppLibertadoresHAS.Services;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace AppLibertadoresHAS.ViewModels
{
    public class LocalizacaoViewModel : BaseViewModel
    {
        
        private UsuarioService uService;
        public LocalizacaoViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            uService = new UsuarioService(token);
        }

        
        private CancellationTokenSource? _cancelTokenSource;
        private bool _isCheckingLocation;

        
        public event Action<Location>? LocalizacaoObtida;

        
        private Map meuMapa;
        public Map MeuMapa
        {
            get => meuMapa;
            set
            {
                if (value != null)
                {
                    meuMapa = value;
                    OnPropertyChanged();
                }
            }
        }

        
        public async void InicializarMapa()
        {
            try
            {
                
                Location location = new Location(-23.5200241d, -46.596498d);
                Pin pinEtec = new Pin()
                {
                    Type = PinType.Place,
                    Label = "Etec Horácio",
                    Address = "Rua alcântara, 113, Vila Guilherme",
                    Location = location
                };

                Map map = new Map();
                MapSpan mapSpan = MapSpan
                    .FromCenterAndRadius(location, Distance.FromKilometers(5));
                map.Pins.Add(pinEtec);
                map.MoveToRegion(mapSpan);

                MeuMapa = map;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                        .DisplayAlertAsync("Erro", ex.Message, "OK");
            }
        }

        
        public async Task ChecarLocalizacao()
        {
            try
            {
                _isCheckingLocation = true;
                _cancelTokenSource = new CancellationTokenSource();
                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);
                if (location != null)
                    LocalizacaoObtida?.Invoke(location);

                Usuario uLoc = new Usuario();
                uLoc.Id = Preferences.Get("UsuarioId", 0);
                uLoc.Latitude = location.Latitude;
                uLoc.Longitude = location.Longitude;

                await uService.PutAtualizarLocalizacaoAsync(uLoc);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlertAsync("Erro", ex.Message, "Ok");
            }
        }

        
        public async Task ExibirUsuariosNoMapa()
        {
            try
            {
                
                ObservableCollection<Usuario> ocUsuarios = await uService.GetUsuariosAsync();
                List<Usuario> listaUsuarios = new List<Usuario>(ocUsuarios);
                Map map = new Map();

                foreach (Usuario u in listaUsuarios)
                {
                    if (u.Latitude != null && u.Longitude != null)
                    {
                        double latitude = (double)u.Latitude;
                        double logitude = (double)u.Longitude;
                        Location location = new Location(latitude, logitude);

                        Pin pinAtual = new Pin()
                        {
                            Type = PinType.Place,
                            Label = u.Username,
                            Address = $"E-mail: {u.Email}",
                            Location = location
                        };
                        map.Pins.Add(pinAtual);
                    }
                }
                MeuMapa = map;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlertAsync("Erro", ex.Message, "OK");
            }
        }

        
        public async void CarregarMapa()
        {
            await ChecarLocalizacao();
            await ExibirUsuariosNoMapa();
        }
    }
}
