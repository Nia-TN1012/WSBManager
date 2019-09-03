using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WSBManager.Models;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Windows.Storage;
using System.Text.RegularExpressions;

namespace WSBManager.ViewModels {

	public enum MappedFolderValidateResult {
		OK, HostFolderPathInvalid, HostFolderNameDuplicated
	}

	class SandboxConfigEditorViewModel : INotifyPropertyChanged {

		WSBManagerModel model;

		private int selectedIndex;

		public WSBConfigManagerModel EditingItem { get; private set; }

		public ObservableCollection<MappedFolder> EditingMappedFolders { get; private set; } = new ObservableCollection<MappedFolder>();

		public bool IsNew { get; private set; }

		public bool IsNewTitleVisible => IsNew;

		public bool IsEditTitleVisible => !IsNew;

		public SandboxConfigEditorViewModel( int selectedIndex = -1 ) {
			model = ( App.Current as App )?.Model;
			if( model == null ) {
				throw new Exception( $"Failed to get reference of model instance on the {GetType()} class." );
			}
			this.selectedIndex = selectedIndex;
			IsNew = selectedIndex <= -1;
			if( IsNew ) {
				EditingItem = new WSBConfigManagerModel();
			}
			else {
				EditingItem = new WSBConfigManagerModel( model.WSBConfigCollection[selectedIndex] );
				foreach( var mf in EditingItem.MappedFolders ) {
					EditingMappedFolders.Add( mf );
				}
			}

			model.PropertyChanged += ( sender, e ) => PropertyChanged?.Invoke( sender, e );
		}

		private static readonly Regex hostFolderReg = new Regex( "^[A-Za-z]:" );

		public ( MappedFolderValidateResult result, string[] validateFailedHostFolders ) Validate() {
			
			var invalidFolders = EditingMappedFolders.Where( item => item.HostFolder == null || !hostFolderReg.IsMatch( item.HostFolder ) );
			if( invalidFolders.Any() ) {
				return ( MappedFolderValidateResult.HostFolderPathInvalid, invalidFolders.Select( item => $"* {item.HostFolder}" ).ToArray() );
			}
			var duplicateFolders = EditingMappedFolders.GroupBy( item => Path.GetFileName( item.HostFolder ) ).Where( item => item.Count() >= 2 );
			if( duplicateFolders.Any() ) {
				return ( MappedFolderValidateResult.HostFolderNameDuplicated, duplicateFolders.SelectMany( item => item.AsEnumerable() ).Select( item => $"* {item.HostFolder}" ).ToArray() );
			}

			return ( MappedFolderValidateResult.OK, null );
		}

		public void Save() {
			EditingItem.MappedFolders.Clear();
			EditingItem.MappedFolders.AddRange( EditingMappedFolders );
			if( IsNew ) {
				model.WSBConfigCollection.Add( EditingItem );
			}
			else {
				model.WSBConfigCollection[selectedIndex] = EditingItem;
			}
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
			
			private SandboxConfigEditorViewModel viewModel;
			
			internal AddMappedFolderCommand( SandboxConfigEditorViewModel _viewModel ) {
				viewModel = _viewModel;
				viewModel.PropertyChanged += ( sender, e ) => CanExecuteChanged?.Invoke( sender, e );
			}

			public bool CanExecute( object parameter ) => true;

			public event EventHandler CanExecuteChanged;

			public void Execute( object parameter ) =>
				viewModel.EditingMappedFolders.Add( new MappedFolder() );
		}

		private class RemoveMappedFolderCommand : ICommand {

			private SandboxConfigEditorViewModel viewModel;

			internal RemoveMappedFolderCommand( SandboxConfigEditorViewModel _viewModel ) {
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
