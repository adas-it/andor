namespace Family.Budget.Application.Dto.Common.Response;
public class KeyValuePairModel<TKey, TValue>
{
    public TKey Key { get; set; }
    public TValue Value { get; set; }
}
