namespace MyGuitarShop.Common.Mappers
{
    public static class AutoReflectionMapper
    {
        public static TTarget? Map<TSource, TTarget>(TSource? source)
            where TTarget : new()
        {
            if(source == null)
            {
                return default;
            }
            var target = new TTarget();
            var sourceProps = typeof(TSource).GetProperties();
            var targetProps = typeof(TTarget).GetProperties();

            foreach(var sourceProp in sourceProps)
            {
                var targetProp = targetProps.FirstOrDefault(p => p.Name == sourceProp.Name &&
                                                                 p.PropertyType == sourceProp.PropertyType &&
                                                                 p.CanWrite);
                if (targetProp == null) continue;
                
                var value = sourceProp.GetValue(source);
                targetProp.SetValue(target, value);                
            }
            return target;
        }
    }
}