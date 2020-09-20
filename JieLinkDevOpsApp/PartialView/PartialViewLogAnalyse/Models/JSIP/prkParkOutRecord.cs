﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewLogAnalyse.Models.JSIP
{
    public class prkParkOutRecord
    {
       
        public int accountReceivable { get; set; }


        public int allBenefit { get; set; }


        public int allCharge { get; set; }


        public int allDerate { get; set; }


        //public prkParkChargeRecord billRecord { get; set; }


        public uint boxId { get; set; }


        public string carColor { get; set; }


        public string carLogo { get; set; }


        public string carNo { get; set; }


        public int carNumColor { get; set; }


        public string carNumOrig { get; set; }


        public int chargeType { get; set; }


        public string credentialNo { get; set; }


        public int credentialType { get; set; }


        public int credibleDegree { get; set; }


        public uint deviceId { get; set; }


        public string deviceName { get; set; }


        public int eventType { get; set; }


        public string inRecordId { get; set; }


        public int ioType { get; set; }


        public string operatorNo { get; set; }


        public comUtcTime operatorTime { get; set; }


        public string[] pictureFilePath { get; set; }


        public prkRecordFlags recordFlags { get; set; }


        public string recordId { get; set; }


        public int sealId { get; set; }


        public string sealName { get; set; }


        public string shareOrderNo { get; set; }


        public comUtcTime time { get; set; }


        public int withHoldMoney { get; set; }


        public string chargeIdList { get; set; }


        public uint sourceDeviceId { get; set; }


        public bool haveCar { get; set; }

        public string guid { get; set; }
    }
}
