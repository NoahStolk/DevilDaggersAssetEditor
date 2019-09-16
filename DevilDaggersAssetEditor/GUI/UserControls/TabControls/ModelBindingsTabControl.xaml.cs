using DevilDaggersAssetCore;
using System.Windows;
using System.Windows.Controls;
using DevilDaggersAssetEditor.GUI.UserControls.AssetControls;
using DevilDaggersAssetEditor.Code.TabControlHandlers;

namespace DevilDaggersAssetEditor.GUI.UserControls.TabControls
{
	public partial class ModelBindingsTabControl : UserControl
	{
		public static readonly DependencyProperty BinaryFileNameProperty = DependencyProperty.Register
		(
			"BinaryFileName",
			typeof(BinaryFileName),
			typeof(ModelBindingsTabControl),
			new PropertyMetadata(BinaryFileName.DD)
		);

		public BinaryFileName BinaryFileName
		{
			get => (BinaryFileName)GetValue(BinaryFileNameProperty);
			set => SetValue(BinaryFileNameProperty, value);
		}

		public ModelBindingsTabControlHandler Handler { get; private set; }

		public ModelBindingsTabControl()
		{
			InitializeComponent();

			Handler = new ModelBindingsTabControlHandler(BinaryFileName);

			foreach (ModelBindingAssetControl ac in Handler.CreateUserControls())
				AssetEditor.Children.Add(ac);
		}
	}
}