using PetShopCare.Models;
using PetShopCare.Repositories;
using PetShopCare.Services; // Assumindo que o seu CriptografiaService está aqui
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PetShopCare.ViewModels {
	public class LoginViewModel : ViewModelBase {
		private readonly UsuarioRepository _usuarioRepository;
		private string _loginUsuario;
		private string _mensagemErro;

		public string LoginUsuario {
			get => _loginUsuario;
			set { _loginUsuario = value; OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); }
		}

		public string MensagemErro {
			get => _mensagemErro;
			set { _mensagemErro = value; OnPropertyChanged(); }
		}

		public ICommand EntrarCommand { get; }
		public ICommand FecharCommand { get; }

		// Ação disparada quando o login for bem-sucedido
		public Action<Usuario> AoAutenticarComSucesso { get; set; }
		public Usuario UsuarioAutenticado { get; private set; }

		public LoginViewModel() {
			_usuarioRepository = new UsuarioRepository();

			EntrarCommand = new RelayCommand(ExecutarEntrar, PodeEntrar);
			FecharCommand = new RelayCommand(ExecutarFechar);
		}

		private bool PodeEntrar(object obj) {
			return !string.IsNullOrWhiteSpace(LoginUsuario);
		}

		private void ExecutarEntrar(object obj) {
			// Recebemos a PasswordBox do XAML via CommandParameter
			if (obj is PasswordBox passwordBox) {
				string senhaDigitada = passwordBox.Password;

				if (string.IsNullOrWhiteSpace(senhaDigitada)) {
					MensagemErro = "Por favor, informe a senha.";
					return;
				}

				// 1. Busca o usuário no banco
				var usuario = _usuarioRepository.BuscarPorLogin(LoginUsuario);

				if (usuario != null) {
					// 2. Compara o hash da senha digitada com o hash salvo no banco
					// NOTA: Ajuste a chamada abaixo para o nome exato do seu método no CriptografiaService
					string hashDigitado = CriptografiaService.GerarHash(senhaDigitada);

					if (usuario.SenhaHash == hashDigitado) {
						// Autenticado com sucesso!
						MensagemErro = string.Empty;
						UsuarioAutenticado = usuario;
						AoAutenticarComSucesso?.Invoke(usuario);
						return;
					}
				}

				// Se chegou aqui, ou o usuário não existe ou a senha está errada
				MensagemErro = "Usuário ou senha inválidos.";
			}
		}

		private void ExecutarFechar(object obj) {
			// Encerra a aplicação completamente se o usuário fechar a tela de login
			Application.Current.Shutdown();
		}
	}
}