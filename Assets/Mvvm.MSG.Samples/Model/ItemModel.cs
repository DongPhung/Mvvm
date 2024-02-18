using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZBase.Foundation.Mvvm.ComponentModel;

namespace Mvvm
{
    public partial class BasicInfo : IObservableObject
    {
        [ObservableProperty] private string _name;
        [ObservableProperty] private string _desciption;
        [ObservableProperty] private string _avatar;
    }

    public partial class ItemModel : IObservableObject
    {
        [ObservableProperty] private BasicInfo _basicInfo = new BasicInfo();
        [ObservableProperty] private int _owned;
        [ObservableProperty] private int _used;
        [ObservableProperty] private int _cost;
    }
}
