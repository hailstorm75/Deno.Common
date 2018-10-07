using PropertyChanged;
using CommonWPFUC.Annotations;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Runtime.CompilerServices;

namespace CommonWPFUC
{
  /// <summary>
  /// Interaction logic for Switch.xaml
  /// </summary>
  [AddINotifyPropertyChangedInterface]
  public partial class Switch : UserControl, INotifyPropertyChanged
  {
    #region Properties

    public bool Enabled
    {
      get => enabled;
      set
      {
        enabled = value;
        EnabledChanged();
      }
    }
    private bool enabled;

    public double Size { get; set; } = 13;
    private double SwitchSize => Size - 2;
    private double DotSize => Size / 3;
    private double CornerRadius => Size / 2;
    private double MainWidth => Size * 2;
    private double MainHeight => Size;
    private Thickness SwitchPostion { get; set; }

    #endregion

    #region Constants

    private readonly Thickness SwitchLeft;
    private readonly Thickness SwitchRight;

    #endregion

    public Switch()
    {
      DataContext = this;

      InitializeComponent();

      SwitchLeft = new Thickness(0, 0, Size, 0);
      SwitchRight = new Thickness(0, 0, -Size, 0);
      SwitchPostion = SwitchLeft;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void EnabledChanged() => SwitchPostion = Enabled ? SwitchLeft : SwitchRight;

    private void UserControl_MouseDown(object sender, MouseButtonEventArgs e) => Enabled = !Enabled;
  }
}
