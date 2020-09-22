namespace GeneralDataLayer.Dynamics.Interfaces
{
    internal interface IDynamicDataInfo
    {
        object GetValue(object obj);

        void SetValue(object obj, object value);
    }
}