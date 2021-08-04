using System.Collections.Generic;

namespace ASPXParser
{
    public class FileExtension
    {
        public const string WebFormsView = ".aspx";
        public const string WebFormsCodeBehind = ".aspx.cs";
        public const string UserControl = ".ascx";
        public const string UserControlCodeBehind = ".ascx.cs";
        public const string MasterFile = ".master";
        public const string MasterFileCodeBehind = ".master.cs";
        public const string Project = ".csproj";

        public static readonly ISet<string> WebFormsFiles = new HashSet<string>
        {
            WebFormsView,
            WebFormsCodeBehind,
            UserControl,
            UserControlCodeBehind,
            MasterFile,
            MasterFileCodeBehind
        };
    }
}