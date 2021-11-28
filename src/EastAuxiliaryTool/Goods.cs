namespace EastAuxiliaryTool
{
    /// <summary>
    /// 货源
    /// </summary>
    public class Goods
    {
        /// <summary>
        /// 货源名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 吨位
        /// </summary>
        public string Weight { get; set; }
        /// <summary>
        /// 出发地
        /// </summary>
        public string Depart { get; set; }
        /// <summary>
        /// 到达地
        /// </summary>
        public string Arrive { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string Person { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Intro { get; set; }
        public string StartYear { get; set; }
        public string StartMonth { get; set; }
        public string StartDay { get; set; }
        public string EndYear { get; set; }
        public string EndMonth { get; set; }
        public string EndDay { get; set; }
    }
}
