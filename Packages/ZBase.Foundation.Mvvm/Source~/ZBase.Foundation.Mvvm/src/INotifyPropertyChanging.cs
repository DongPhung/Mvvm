﻿namespace ZBase.Foundation.Mvvm
{
    /// <summary>
    /// Notifies clients that a property value is changing.
    /// </summary>
    public interface INotifyPropertyChanging
    {
        bool PropertyChanging<TInstance>(string propertyName, PropertyChangeEventListener<TInstance> listener)
            where TInstance : class;
    }
}
