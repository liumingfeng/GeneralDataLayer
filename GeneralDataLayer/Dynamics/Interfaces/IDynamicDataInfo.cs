namespace GeneralDataLayer.Dynamics.Interfaces
{
    public interface IDynamicDataInfo
    {
        object GetValue(object obj);

        void SetValue(object obj, object value);
    }
}