using System;
using System.Collections.Generic;

namespace SKCalendar
{
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot("calendar")]
    public class SKCalendarDates
    {
        [XmlElement("calendarDate")]
        public List<SKCalendarDate> Dates { get; set; }
    }

    [Serializable]
    public class SKCalendarDate
    {
        [XmlAttribute("date")]
        public string Date { get; set; }

        [XmlAttribute("namedays")]
        public string Namedays { get; set; }

        [XmlAttribute("holidays")]
        public string Holiday { get; set; }

        [XmlAttribute("worldDay")]
        public string WorldDay { get; set; }
    }
}
