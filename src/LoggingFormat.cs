using Serilog.Events;
using Serilog.Formatting;
using System;
using System.Buffers;
using System.IO;
using System.Text;
using System.Text.Json;

namespace MakeSimple.Logging
{
    public class LoggingFormat : ITextFormatter
    {
        /// <summary>
        /// Format the log event into the output. Subsequent events will be newline-delimited.
        /// </summary>
        /// <param name="logEvent">The event to format.</param>
        /// <param name="output">The output.</param>
        public void Format(LogEvent logEvent, TextWriter output)
        {
            FormatEvent(logEvent, output);
            output.WriteLine();
        }

        /// <summary>
        /// Format the log event into the output.
        /// </summary>
        /// <param name="logEvent">The event to format.</param>
        /// <param name="output">The output.</param>
        /// <param name="valueFormatter">A value formatter for <see cref="LogEventPropertyValue"/>s on the event.</param>
        public static void FormatEvent(LogEvent logEvent, TextWriter output)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
            if (output == null) throw new ArgumentNullException(nameof(output));

            logEvent.AddOrUpdateProperty(new LogEventProperty("Level", new ScalarValue(logEvent.Level.ToString())));
            if (logEvent.Exception != null)
                logEvent.AddOrUpdateProperty(new LogEventProperty("Exception", new ScalarValue(logEvent.Exception.ToString())));

            string message = logEvent.MessageTemplate.Render(logEvent.Properties);
            var isPaser = JsonTryPaser(message, out JsonDocument document);
            if (!isPaser)
            {
                logEvent.AddOrUpdateProperty(new LogEventProperty("Message", new ScalarValue(message)));
            }

            var outputBuffer = new ArrayBufferWriter<byte>();
            using (var jDoc = document)
            using (var jsonWriter = new Utf8JsonWriter(outputBuffer, new JsonWriterOptions { Indented = false, SkipValidation = true }))
            {
                jsonWriter.WriteStartObject();
                if (jDoc != null)
                    foreach (JsonProperty property in jDoc.RootElement.EnumerateObject())
                    {
                        if (!logEvent.Properties.ContainsKey(property.Name))
                        {
                            property.WriteTo(jsonWriter);
                        }
                    }

                foreach (var property in logEvent.Properties)
                {
                    jsonWriter.WriteString(property.Key, property.Value.ToString());
                }
                jsonWriter.WriteEndObject();
            }
            output.Write(Encoding.UTF8.GetString(outputBuffer.WrittenSpan));
        }

        private static bool JsonTryPaser(string log, out JsonDocument output)
        {
            log = log.Trim();
            if ((log.StartsWith("{") && log.EndsWith("}")) || //For object
                (log.StartsWith("[") && log.EndsWith("]"))) //For array
            {
                try
                {
                    output = JsonDocument.Parse(log);
                    return true;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    output = null;
                    return false;
                }
            }
            output = null;
            return false;
        }
    }
}