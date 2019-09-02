using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WSBManager.Models;

namespace WSBManager.ViewModels {
	class SandboxConfigCreatorViewModel : INotifyPropertyChanged {
		WSBManagerModel model;

		public WSBConfigManagerModel EditingItem { get; private set; }

		public ObservableCollection<MappedFolder> EditingMappedFolders { get; private set; } = new ObservableCollection<MappedFolder>();

		public SandboxConfigCreatorViewModel() {
			model = ( App.Current as App )?.Model;
			if( model == null ) {
				throw new Exception( $"Failed to get reference of model instance on the {GetType()} class." );
			}

			EditingItem = new WSBConfigManagerModel();
			model.PropertyChanged += ( sender, e ) => PropertyChanged?.Invoke( sender, e );
		}

		public void Save() {
			EditingItem.MappedFolders.AddRange( EditingMappedFolders );
			model.WSBConfigCollection.Add( EditingItem );
		}

		/// <summary>
		///	The event handler to be generated after the property changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		///	Notifies the property change that corresponds to the specified property name.
		/// </summary>
		/// <param name="propertyName">Property name</param>
		private void NotifyPropertyChanged( [CallerMemberName]string propertyName = null ) =>
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );

		private ICommand addMappedFolder;
		public ICommand AddMappedFolder =>
			addMappedFolder ?? ( addMappedFolder = new AddMappedFolderCommand( this ) );

		private ICommand removeMappedFolder;
		public ICommand RemoveMappedFolder =>
			removeMappedFolder ?? ( removeMappedFolder = new RemoveMappedFolderCommand( this ) );

		private class AddMappedFolderCommand : ICommand {

			private SandboxConfigCreatorViewModel viewModel;


			internal AddMappedFolderCommand( SandboxConfigCreatorViewModel _viewModel ) {
				viewModel = _viewModel;
				viewModel.PropertyChanged += ( sender, e ) => CanExecuteChanged?.Invoke( sender, e );
			}


			public bool CanExecute( object parameter ) => true;

			public event EventHandler CanExecuteChanged;

			public void Execute( object parameter ) =>
				viewModel.EditingMappedFolders.Add( new MappedFolder() );
		}

		private class RemoveMappedFolderCommand : ICommand {

			private SandboxConfigCreatorViewModel viewModel;


			internal RemoveMappedFolderCommand( SandboxConfigCreatorViewModel _viewModel ) {
				viewModel = _viewModel;
				viewModel.PropertyChanged += ( sender, e ) => CanExecuteChanged?.Invoke( sender, e );
			}


			public bool CanExecute( object parameter ) => true;

			public event EventHandler CanExecuteChanged;

			public void Execute( object parameter ) {
				if( parameter is int selected && selected > -1 ) {
					viewModel.EditingMappedFolders.RemoveAt( selected );
				}
			}
		}
	}
}
