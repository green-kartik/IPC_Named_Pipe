using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FileMappingInterface
{
	public static class FileMappingConsts
	{
		public const string MUTEX = "Global\\Test1"; // not required // done automatically
		public const string SERVER_LOCATION = ".";
		public const string PIPE_ONE_ClIENT_READ_SERVER_WRITE = "20AC0389-8549-4FFB-BD2B-0BD0E7ECEE58";
		public const string PIPE_TWO_CLIENT_WRITE_SERVER_READ = "F83AF978-93FC-4055-B410-AA5C81AB01DF";
	}
}
