using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY.SpiderService.Module
{
    public class JsonObject
    {
        public string status { get; set; }
        public List<PiaoFang> data { get; set; }

        public pageable pageable { get; set; }
    }

    public class JsonObjectTing
    {
        public string status { get; set; }
        public List<PiaoFangTing> data { get; set; }

        public pageable pageable { get; set; }
    }
    public class JsonObjectYingYuan
    {
        public string status { get; set; }
        public List<YingYuan> data { get; set; }

        public pageable pageable { get; set; }
    }

    public class YingYuan
    {
        public string cinemaCode { get; set; }
        public string cinemaName { get; set; }
        public string provinceCode { get; set; }
        public string provinceName { get; set; }
        public string officalName { get; set; }
        public string cinemaChainName { get; set; }
        public string pcsell { get; set; }
        public string softwareName { get; set; }
        public string businessStatus { get; set; }
        public string totalScreenCount { get; set; }
        public string totalSeatCount { get; set; }
        public string businessDate { get; set; }
        public string secondApprovalTime { get; set; }
        public string totalFilm { get; set; }
        public string totalSession { get; set; }
        public string totalAudience { get; set; }
        public string totalBoxOffice { get; set; }
        public string totalService { get; set; }
        public string totalBusinessDays { get; set; }
        public string reportDays { get; set; }
    }

    public class PiaoFangTing
    {
        public string provinceName { get; set; }
        public string cinemaChainName { get; set; }
        public string cinemaCode { get; set; }
        public string cinemaName { get; set; }
        public string filmCode { get; set; }
        public string filmName { get; set; }
        public string publicVerName { get; set; }
        public string seatCount { get; set; }
        public string businessDate { get; set; }
        public string sessionDate { get; set; }
        public string sessionCode { get; set; }
        public string sessionTime { get; set; }
        public string reportTime { get; set; }
        public string sessionDateTime { get; set; }
        public string localSalesCount { get; set; }
        public string localSales { get; set; }
        public string onlineSalesCount { get; set; }
        public string onlineSales { get; set; }
        public string localRefundCount { get; set; }
        public string localRefund { get; set; }
        public string onlineRefundCount { get; set; }
        public string onlineRefund { get; set; }
        public string pastSaleCount { get; set; }
        public string pastSales { get; set; }
        public string totalAudience { get; set; }
        public string totalBoxOffice { get; set; }
        public string totalBoxOfficeAndService { get; set; }
        public string totalService { get; set; }
        public string screenCode { get; set; }
        public string screenName { get; set; }
    }

    public class PiaoFang
    {
        public string provinceName { get; set; }
        public string cinemaCode { get; set; }
        public string cinemaName { get; set; }
        public string filmCode { get; set; }
        public string filmName { get; set; }
        public string filmVersion { get; set; }
        public string totalSession { get; set; }
        public string totalAudience { get; set; }
        public string totalBoxoffice { get; set; }
        public string totalService { get; set; }
        public string businessDate { get; set; }
    }

    public class pageable
    {
        public bool last { get; set; }
        public int totalPages { get; set; }

        public int totalElements { get; set; }
    }
}