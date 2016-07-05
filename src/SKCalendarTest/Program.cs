namespace SKCalendarTest
{
    using System;
    using System.Text.RegularExpressions;

    using SKCalendar;

    class Program
    {
        static void Main(string[] args)
        {
            var calendar = new SKCalendar();
            Console.WriteLine(calendar.RenderCalendarInfo(DateTime.Now, "Dnes je {longdate}{nameday:, Meniny má [value]}.<!--TOOLTIP-->{holiday:<br /><strong>[value]</strong>}{worldday:<br />[value]}."));
            Console.ReadLine();
        }
    }
}
