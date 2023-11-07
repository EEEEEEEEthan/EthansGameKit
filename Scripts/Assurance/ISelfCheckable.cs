using System.Collections.Generic;

namespace EthansGameKit.Assurance
{
	public interface ISelfCheckable
	{
		IEnumerable<Problem> SelfCheck();
	}
}
