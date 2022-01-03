using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;

using WSBManager.Models;

namespace WSBManager.ViewModels
{

	/// <summary>
	/// A validation result
	/// </summary>
	public enum MappedFolderValidateResult
	{
		OK, HostFolderPathInvalid, HostFolderNameDuplicated
	}

	/// <summary>
	/// Sandbox configuration editor view model
	/// </summary>
	class SandboxConfigEditorViewModel : INotifyPropertyChanged
	{
		/// <summary>
		/// Resource loader
		/// </summary>
		private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();

		/// <summary>
		/// Model
		/// </summary>
		private readonly WSBManagerModel model;

		/// <summary>
		/// Selected item in the Main page's list.
		/// </summary>
		private readonly int selectedIndex;

		/// <summary>
		/// 
		/// </summary>
		public WSBConfigManagerModel EditingItem { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public ObservableCollection<MappedFolder> EditingMappedFolders { get; private set; } = new ObservableCollection<MappedFolder>();

		/// <summary>
		/// Whether the configuration item being edited is newly created
		/// </summary>
		public bool IsNew { get; private set; }

		/// <summary>
		/// Whether to display the title 'new creation'
		/// </summary>
		public bool IsNewTitleVisible => IsNew;

		/// <summary>
		/// Whether to display the title 'editing'
		/// </summary>
		public bool IsEditTitleVisible => !IsNew;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="selectedIndex">Index of the configuration item to edit (if -1, new creattion)</param>
		public SandboxConfigEditorViewModel(int selectedIndex = -1)
		{
			model = (App.Current as App)?.Model;
			if (model == null)
			{
				throw new Exception($"Failed to get reference of model instance on the {GetType()} class.");
			}
			this.selectedIndex = selectedIndex;
			IsNew = selectedIndex <= -1;
			if (IsNew)
			{
				EditingItem = new WSBConfigManagerModel { Name = resourceLoader.GetString("NewSandboxName") };
			}
			else
			{
				EditingItem = new WSBConfigManagerModel(model.WSBConfigCollection[selectedIndex]);
				foreach (var mf in EditingItem.MappedFolders)
				{
					EditingMappedFolders.Add(mf);
				}
			}

			model.PropertyChanged += (sender, e) => PropertyChanged?.Invoke(sender, e);
		}

		/// <summary>
		/// Regular expression for host folder validation check
		/// </summary>
		private static readonly Regex hostFolderReg = new Regex("^[A-Za-z]:");

		public (MappedFolderValidateResult result, string[] validateFailedHostFolders) Validate()
		{

			var invalidFolders = EditingMappedFolders.Where(item => item.HostFolder == null || !hostFolderReg.IsMatch(item.HostFolder));
			if (invalidFolders.Any())
			{
				return (MappedFolderValidateResult.HostFolderPathInvalid, invalidFolders.Select(item => $"* {item.HostFolder}").ToArray());
			}
			var duplicateFolders = EditingMappedFolders.GroupBy(item => Path.GetFileName(item.HostFolder)).Where(item => item.Count() >= 2);
			if (duplicateFolders.Any())
			{
				return (MappedFolderValidateResult.HostFolderNameDuplicated, duplicateFolders.SelectMany(item => item.AsEnumerable()).Select(item => $"* {item.HostFolder}").ToArray());
			}

			return (MappedFolderValidateResult.OK, null);
		}

		/// <summary>
		/// Reflects the edited contents in the list.
		/// </summary>
		public void Save()
		{
			EditingItem.MappedFolders.Clear();
			EditingItem.MappedFolders.AddRange(EditingMappedFolders);
			if (IsNew)
			{
				model.WSBConfigCollection.Add(EditingItem);
			}
			else
			{
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
		private void NotifyPropertyChanged([CallerMemberName] string propertyName = null) =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		private ICommand addMappedFolder;
		/// <summary>
		/// Add mapped folder <see cref="ICommand"/> object.
		/// </summary>
		public ICommand AddMappedFolder =>
			addMappedFolder ?? (addMappedFolder = new AddMappedFolderCommand(this));

		private ICommand removeMappedFolder;
		/// <summary>
		/// Remove mapped folder <see cref="ICommand"/> object.
		/// </summary>
		public ICommand RemoveMappedFolder =>
			removeMappedFolder ?? (removeMappedFolder = new RemoveMappedFolderCommand(this));

		/// <summary>
		/// Add mapped folder command
		/// </summary>
		private class AddMappedFolderCommand : ICommand
		{

			/// <summary>
			/// View model referenece
			/// </summary>
			private readonly SandboxConfigEditorViewModel viewModel;

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="_viewModel">View model</param>
			internal AddMappedFolderCommand(SandboxConfigEditorViewModel _viewModel)
			{
				viewModel = _viewModel;
				viewModel.PropertyChanged += (sender, e) => CanExecuteChanged?.Invoke(sender, e);
			}

			public bool CanExecute(object parameter) => true;

			public event EventHandler CanExecuteChanged;

			public void Execute(object parameter)
			{
				viewModel.EditingMappedFolders.Add(new MappedFolder());
			}
		}

		/// <summary>
		/// Remove mapped folder command
		/// </summary>
		private class RemoveMappedFolderCommand : ICommand
		{

			private readonly SandboxConfigEditorViewModel viewModel;

			internal RemoveMappedFolderCommand(SandboxConfigEditorViewModel _viewModel)
			{
				viewModel = _viewModel;
				viewModel.PropertyChanged += (sender, e) => CanExecuteChanged?.Invoke(sender, e);
			}


			public bool CanExecute(object parameter) => viewModel.EditingMappedFolders.Any();

			public event EventHandler CanExecuteChanged;

			public void Execute(object parameter)
			{
				if (parameter is int selected && selected > -1)
				{
					viewModel.EditingMappedFolders.RemoveAt(selected);
				}
			}
		}
	}
}
