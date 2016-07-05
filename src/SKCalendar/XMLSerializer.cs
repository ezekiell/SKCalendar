using System;

namespace SKCalendar
{
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    public class XMLSerializer
    {
        /// <summary>
        /// Serializes the settings to the xml file on specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="dates">The collection of dates.</param>
        public static void Serialize<T>(string path, T dates)
        {
            try
            {
                using (TextWriter textWriter = new StreamWriter(path))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(textWriter, dates);
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException("Serialization failed!", ex);
            }
        }

        /// <summary>
        /// Deserializes the settings object from the xml file onthe specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static T Deserialize<T>(string path)
        {
            try
            {
                using (TextReader textReader = new StreamReader(path))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    return (T)xmlSerializer.Deserialize(textReader);
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException("Deserialization failed!", ex);
            }
        }

        /// <summary>
        /// Deserializes the settings object from the xml resource file.
        /// </summary>
        /// <returns>
        /// Deserialized resource file dates.xml
        /// </returns>
        public static T Deserialize<T>()
        {
            try
            {
                using (TextReader textReader = new StringReader(Resource.dates))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    return (T)xmlSerializer.Deserialize(textReader);
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException("Deserialization failed!", ex);
            }
        }
    }
}
