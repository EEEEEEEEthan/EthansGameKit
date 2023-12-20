namespace EthansGameKit.Collections.Wrappers
{
	public interface IConverter<in TOld, out TNew>
	{
		TNew Convert(TOld old);
	}
}
