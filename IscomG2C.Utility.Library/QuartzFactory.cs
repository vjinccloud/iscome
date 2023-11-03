using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;

namespace IscomG2C.Utility.Library
{
    public class QuartzFactory
    {
        // 調度器工廠
        private static ISchedulerFactory factory = null;
        // 調度器
        private static IScheduler scheduler = null;

        /// <summary>
        /// 建構式
        /// </summary>
        static QuartzFactory()
        {
            factory = new StdSchedulerFactory();
            scheduler = factory.GetScheduler().Result;
            scheduler.Start();
        }

        #region 觸發器1：添加Job
        /// <summary>
        /// 觸發器1：添加Job并以周期的形式運行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jobName"></param>
        /// <param name="startTime"></param>
        /// <param name="simpleTime"></param>
        /// <param name="jobDataMap"></param>
        /// <returns></returns>
        public static DateTimeOffset AddJob<T>(string jobName, DateTimeOffset startTime, TimeSpan simpleTime, Dictionary<string, object> jobDataMap) where T : IJob
        {
            IJobDetail jobCheck = JobBuilder.Create<T>().WithIdentity(jobName, jobName + "_Group").Build();
            jobCheck.JobDataMap.PutAll(jobDataMap);
            ISimpleTrigger triggerCheck = new SimpleTriggerImpl(jobName + "_SimpleTrigger",
                jobName + "_TriggerGroup",
                startTime,
                null,
                SimpleTriggerImpl.RepeatIndefinitely,
                simpleTime);
            return scheduler.ScheduleJob(jobCheck, triggerCheck).Result;
        }

        /// <summary>
        /// 觸發器1：添加Job并以周期的形式運行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jobName"></param>
        /// <param name="startTime"></param>
        /// <param name="simpleTime">毫秒數</param>
        /// <param name="mapKey"></param>
        /// <param name="mapValue"></param>
        /// <returns></returns>
        public static DateTimeOffset AddJob<T>(string jobName, DateTimeOffset startTime, int simpleTime, string mapKey, object mapValue) where T : IJob
        {
            Dictionary<string, object> jobDataMap = new Dictionary<string, object>
            {
                { mapKey, mapValue }
            };
            return AddJob<T>(jobName, startTime, TimeSpan.FromMilliseconds(simpleTime), jobDataMap);
        }

        /// <summary>
        /// 觸發器1：添加Job并以周期的形式運行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jobName"></param>
        /// <param name="startTime"></param>
        /// <param name="simpleTime"></param>
        /// <returns></returns>
        public static DateTimeOffset AddJob<T>(string jobName, DateTimeOffset startTime, TimeSpan simpleTime) where T : IJob
        {
            return AddJob<T>(jobName, startTime, simpleTime, new Dictionary<string, object>());
        }

        /// <summary>
        /// 觸發器1：添加Job并以周期的形式運行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jobName"></param>
        /// <param name="startTime"></param>
        /// <param name="simpleTime">毫秒數</param>
        /// <returns></returns>
        public static DateTimeOffset AddJob<T>(string jobName, DateTimeOffset startTime, int simpleTime) where T : IJob
        {
            return AddJob<T>(jobName, startTime, TimeSpan.FromMilliseconds(simpleTime));
        }

        /// <summary>
        /// 觸發器1：添加Job并以周期的形式運行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jobName"></param>
        /// <param name="simpleTime">毫秒數</param>
        /// <returns></returns>
        public static DateTimeOffset AddJob<T>(string jobName, int simpleTime) where T : IJob
        {
            return AddJob<T>(jobName, DateTime.UtcNow.AddMilliseconds(1), TimeSpan.FromMilliseconds(simpleTime));
        }
        #endregion

        #region 觸發器4：添加Job
        /// <summary>
        /// 觸發器4：添加Job并以定點的形式運行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jobName"></param>
        /// <param name="cronTime"></param>
        /// <param name="jobDataMap"></param>
        /// <returns></returns>
        public static DateTimeOffset AddJob<T>(string jobName, string cronTime, string jobData) where T : IJob
        {
            IJobDetail jobCheck = JobBuilder.Create<T>().WithIdentity(jobName, jobName + "_Group").UsingJobData("jobData", jobData).Build();
            ICronTrigger cronTrigger = new CronTriggerImpl(jobName + "_CronTrigger", jobName + "_TriggerGroup", cronTime);
            return scheduler.ScheduleJob(jobCheck, cronTrigger).Result;
        }

        /// <summary>
        /// 觸發器4：添加Job并以定點的形式運行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jobName"></param>
        /// <param name="cronTime"></param>
        /// <returns></returns>
        public static DateTimeOffset AddJob<T>(string jobName, string cronTime) where T : IJob
        {
            return AddJob<T>(jobName, cronTime, null);
        }
        #endregion

        /// <summary>
        /// 修改觸發器時間多載
        /// </summary>
        /// <param name="jobName">Job名稱</param>
        /// <param name="timeSpan">TimeSpan</param>
        /// </summary>
        public static void UpdateTime(string jobName, TimeSpan simpleTimeSpan)
        {
            TriggerKey triggerKey = new TriggerKey(jobName + "_SimpleTrigger", jobName + "_TriggerGroup");
            SimpleTriggerImpl simpleTriggerImpl = scheduler.GetTrigger(triggerKey).Result as SimpleTriggerImpl;
            simpleTriggerImpl.RepeatInterval = simpleTimeSpan;
            scheduler.RescheduleJob(triggerKey, simpleTriggerImpl);
        }

        /// <summary>
        /// 修改觸發器時間多載
        /// </summary>
        /// <param name="jobName">Job名稱</param>
        /// <param name="simpleTime">分鐘數</param>
        /// <summary>
        public static void UpdateTime(string jobName, int simpleTime)
        {
            UpdateTime(jobName, TimeSpan.FromMinutes(simpleTime));
        }

        /// <summary>
        /// 修改觸發器時間多載
        /// </summary>
        /// <param name="jobName">Job名稱</param>
        /// <param name="cronTime">Cron運算式</param>
        public static void UpdateTime(string jobName, string cronTime)
        {
            TriggerKey triggerKey = new TriggerKey(jobName + "_CronTrigger", jobName + "_TriggerGroup");
            CronTriggerImpl cronTriggerImpl = scheduler.GetTrigger(triggerKey).Result as CronTriggerImpl;
            cronTriggerImpl.CronExpression = new CronExpression(cronTime);
            scheduler.RescheduleJob(triggerKey, cronTriggerImpl);
        }

        /// <summary>
        /// 暫停所有Job
        /// </summary>
        public static void PauseAll()
        {
            scheduler.PauseAll();
        }

        /// <summary>
        /// 恢復所有Job
        /// </summary>
        public static void ResumeAll()
        {
            scheduler.ResumeAll();
        }

        /// <summary>
        /// 洗掉指定Job
        /// </summary>
        /// <param name="jobName"></param>
        public static void DeleteJob(string jobName)
        {
            JobKey jobKey = new JobKey(jobName, jobName + "_Group");
            scheduler.DeleteJob(jobKey);
        }

        /// <summary>
        /// 卸載定時器
        /// </summary>
        /// <param name="isWaitForToComplete">是否等待Job執行完成</param>
        public static void Shutdown(bool isWaitForToComplete)
        {
            scheduler.Shutdown(isWaitForToComplete);
        }
    }
}
