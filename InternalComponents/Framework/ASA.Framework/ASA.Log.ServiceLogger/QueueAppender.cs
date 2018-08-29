///////////////////////////////////////////////////////////////////////////////
//
//  WorkFile Name:	QueueAppender.cs
//
//  ASA Proprietary Information
//
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using log4net;
using log4net.Core;
using log4net.Appender;
using Spring.Messaging.Nms.Core;
using Apache.NMS.ActiveMQ;

namespace ASA.Log.ServiceLogger
{
    public class QueueAppender : AppenderSkeleton
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private int QueueFailureCount;
        private DateTime LastQueueFailureTime;

        public QueueAppender() : base()
        {
            m_QueueFailureCountThreshold = 3;
            m_QueueRestoreTimeThreshold = 60;

            QueueFailureCount = 0;
            LastQueueFailureTime = DateTime.Now;
        }

        /// <summary>
        /// Send log message to message queue.
        /// </summary>
        protected override void Append(LoggingEvent loggingEvent)
        {
            string text = RenderLoggingEvent(loggingEvent);
            string message = text.Replace("(null)", "");

            if (!MessageValidation(message))
                return;

            bool QueueFailureCountThresholdReached = (QueueFailureCount >= m_QueueFailureCountThreshold);
            bool QueueRestoreTimeThresholdReached;
            if (!QueueFailureCountThresholdReached)
                QueueRestoreTimeThresholdReached = false;
            else
            {
                TimeSpan t = System.DateTime.Now - LastQueueFailureTime;
                QueueRestoreTimeThresholdReached = t.TotalSeconds > m_QueueRestoreTimeThreshold;
            }

            if (!QueueFailureCountThresholdReached || QueueRestoreTimeThresholdReached)
            {
                if (QueueRestoreTimeThresholdReached)
                {
                    // Queue Restore Time Threshold is reached. 
                    // It's time to clear the failure count and direct message to queue.
                    QueueFailureCount = 0;
                    log.Debug("Queue Restore Time Threshold (" + m_QueueRestoreTimeThreshold.ToString() + ") reached. Message is directed to queue.");
                }

                try
                {
                    ConnectionFactory connectionFactory = new ConnectionFactory(m_QueueURI);
                    NmsTemplate template = new NmsTemplate(connectionFactory);
                    template.ConvertAndSend(m_QueueName, message);
                    QueueFailureCount = 0;
                }
                catch (Exception e)
                {
                    log.Fatal("An error occurred while sending message to queue.", e);
                    QueueHelper.RecordOriginalMessage(message);
                    QueueFailureCount++;
                    LastQueueFailureTime = DateTime.Now;
                }
            }
            else
            {
                log.Debug("Queue Failure Count Threshold (" + m_QueueFailureCountThreshold.ToString() + ") reached. Message is directed to local file. ");
                QueueHelper.RecordOriginalMessage(message);
            }
        }

        /// <summary>
        /// Check if message in good XML format.
        /// </summary>
        protected bool MessageValidation(string message)
        {
            bool result = true;
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.LoadXml(message);
            }
            catch (Exception e)
            {
                log.Fatal("Message format error.", e);
                QueueHelper.RecordOriginalMessage(message);
                result = false;
            }
            return result;
        }

        private string m_QueueURI;

        /// <summary>
        /// Queue Address Property.
        /// </summary>
        public string QueueURI
        {
            get { return m_QueueURI; }
            set { m_QueueURI = value; }
        }

        private string m_QueueName;

        /// <summary>
        /// Queue Name Property.
        /// </summary>
        public string QueueName
        {
            get { return m_QueueName; }
            set { m_QueueName = value; }
        }

        /// <summary>
        /// Maximum queue failure count before sending message to local file.
        /// </summary>
        private int m_QueueFailureCountThreshold;

        /// <summary>
        /// Queue failure count threshold; when reached message is directed to local file.
        /// </summary>
        public int QueueFailureCountThreshold
        {
            get { return m_QueueFailureCountThreshold; }
            set { m_QueueFailureCountThreshold = value; }
        }

        /// <summary>
        /// Time (in second) waited before sending message to queue, after the last queue failure.
        /// </summary>
        private int m_QueueRestoreTimeThreshold;

        /// <summary>
        /// Time (in second) waited before sending message to queue, after the last queue failure.
        /// </summary>
        public int QueueRestoreTimeThreshold
        {
            get { return m_QueueRestoreTimeThreshold; }
            set { m_QueueRestoreTimeThreshold = value; }
        }
    }
}
