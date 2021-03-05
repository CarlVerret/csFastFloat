using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Running;

using BenchmarkDotNet.Reports;
using System.IO;
using System.Linq;


namespace csFastFloat.Benchmark
{

  public class MFloatPerSecColumn : IColumn
    {
        public string Id => nameof(MFloatPerSecColumn);

        public string ColumnName => "MFloat/s";

        public string Legend => "Million of floats parsed per second";

        public UnitType UnitType => UnitType.Size;

        public bool AlwaysShow => true;

        public ColumnCategory Category => ColumnCategory.Metric;

        public int PriorityInCategory => 1;

        public bool IsNumeric => true;

        public bool IsAvailable(Summary summary) => true;

        public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;

        public string GetValue(Summary summary, BenchmarkCase benchmarkCase) => GetValue(summary, benchmarkCase, SummaryStyle.Default);

        public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style)
        {
		    var disp_info = benchmarkCase.DisplayInfo;
			
            var fileName = benchmarkCase.Parameters.Items.FirstOrDefault(x => x.Name == "FileName").ToString();


            var nbFloat = System.IO.File.ReadAllLines(fileName).Count();

			var s= summary.Reports.Where(x => x.BenchmarkCase.DisplayInfo == disp_info).FirstOrDefault();
			double fps = nbFloat * 1000 / s.ResultStatistics.Min;

			return string.Format("{0,8:f2}",fps);


        }

        public override string ToString() => ColumnName;
    }



    public class VolumePerSecColumn : IColumn
    {
        public string Id => nameof(VolumePerSecColumn);

        public string ColumnName => "MB/s";

        public string Legend => "Volume of data parsed per second (in MB)";

        public UnitType UnitType => UnitType.Size;

        public bool AlwaysShow => true;

        public ColumnCategory Category => ColumnCategory.Metric;

        public int PriorityInCategory => 2;

        public bool IsNumeric => true;

        public bool IsAvailable(Summary summary) => true;

        public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;

        public string GetValue(Summary summary, BenchmarkCase benchmarkCase) => GetValue(summary, benchmarkCase, SummaryStyle.Default);

        public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style)
        {
		    var disp_info = benchmarkCase.DisplayInfo;
			
            var fileName = benchmarkCase.Parameters.Items.FirstOrDefault(x => x.Name == "FileName").ToString();
           var volume = new FileInfo(fileName).Length/1024;
     
			var s= summary.Reports.Where(x => x.BenchmarkCase.DisplayInfo == disp_info).FirstOrDefault();
			double vps = volume * 1000000 / s.ResultStatistics.Min;

			return string.Format("{0,8:f2}",vps);


        }

        public override string ToString() => ColumnName;
    }

}