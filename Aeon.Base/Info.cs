using System;
using System.Collections.Generic;
using System.Text;

namespace Aeon.Base
{
	public class HeroInfo
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string AssemblyName { get; set; }
	}

	public class HeroSelection
	{
		public string Nickname { get; set; }
		public HeroInfo Hero { get; set; }
	}
}
