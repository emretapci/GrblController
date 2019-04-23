namespace CncController
{
	internal class Parameters
	{
		internal double StartOffset { get; set; } //mm
		internal double Table1Length { get; set; } //mm
		internal double MiddleGap { get; set; } //mm
		internal double Table2Length { get; set; } //mm
		internal double EndOffset { get; set; } //mm

		internal static Parameters ReadFromFile()
		{
			Properties.Settings.Default.Reload();

			return new Parameters()
			{
				StartOffset = (double)Properties.Settings.Default["StartOffset"],
				Table1Length = (double)Properties.Settings.Default["Table1Length"],
				MiddleGap = (double)Properties.Settings.Default["MiddleGap"],
				Table2Length = (double)Properties.Settings.Default["Table2Length"],
				EndOffset = (double)Properties.Settings.Default["EndOffset"],
			};
		}

		internal static void WriteToFile(Parameters parameters)
		{
			Properties.Settings.Default["StartOffset"] = parameters.StartOffset;
			Properties.Settings.Default["Table1Length"] = parameters.Table1Length;
			Properties.Settings.Default["MiddleGap"] = parameters.MiddleGap;
			Properties.Settings.Default["Table2Length"] = parameters.Table2Length;
			Properties.Settings.Default["EndOffset"] = parameters.EndOffset;
			Properties.Settings.Default.Save();
		}
	}
}
