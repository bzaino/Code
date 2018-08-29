using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ASA.Web.Services.Common;

namespace ASA.Web.Services.AppToolService.Proxy.DataContracts
{
    //        <xsd:element name="AppToolInfoType" minOccurs="0">
    //            <xsd:complexType>
    //                <xsd:annotation>
    //                    <xsd:documentation>
    //                        Contains information related to an Application/tool
    //                    </xsd:documentation>
    //                </xsd:annotation>
    //                <xsd:sequence>
    //                    <xsd:element name="AppToolPersonId" type="xsd:int" minOccurs="0" />
    //                    <xsd:element name="AppToolType" type="apptool:ApplicationToolType" />
    //                    <xsd:element name="PrincipalAmount" type="cmn:MoneyType" minOccurs="0" />
    //                    <xsd:element name="InterestRate" type="xsd:double" minOccurs="0" />
    //                    <xsd:element name="NumberOfPayments" type="xsd:int" minOccurs="0" />
    //                    <xsd:element name="MonthlyIncome" type="cmn:MoneyType"  minOccurs="0" />
    //                    <xsd:element name="NumberOfMonthsInForbearance" type="xsd:int"  minOccurs="0" />
    //                    <xsd:element name="ForbearancePaymentAmount" type="cmn:MoneyType"  minOccurs="0" />
    //                    <xsd:element name="NumberOfForbearancePayments" type="xsd:int"  minOccurs="0" />
    //                    <xsd:element name="BalanceAtStartDeferment" type="cmn:MoneyType"  minOccurs="0" />
    //                    <xsd:element name="NumberOfMonthsInDeferment" type="xsd:int"  minOccurs="0" />
    //                    <xsd:element name="RecordVersion" type="xsd:base64Binary" nillable="true" minOccurs="0"/>
    //                </xsd:sequence>
    //            </xsd:complexType>
    //        </xsd:element>
	
    //<xsd:simpleType name="ApplicationToolType">
    //    <xsd:annotation>
    //        <xsd:documentation>
    //            App/Tool type. Should match the one of the enumeration.
							
    //            SRC - Standard Repayment Calculator
    //            GRC - Graduated Repayment Calculator
    //            IRC - Income-sensative Repayment Calculator
    //            DFC - Deferment Calculator
    //            FBC - Forbearance Calculator
									
    //        </xsd:documentation>
    //    </xsd:annotation>
    //    <xsd:restriction base="xsd:string">
    //        <xsd:enumeration value="SRC"/>
    //        <xsd:enumeration value="GRC"/>
    //        <xsd:enumeration value="IRC"/>
    //        <xsd:enumeration value="DFC"/>
    //        <xsd:enumeration value="FBC"/>
    //    </xsd:restriction>
    //</xsd:simpleType>

    public class AppToolModel : BaseWebModel
    {
        [Required]
        [DisplayName("AppToolPerson ID")]
        [DefaultValue(0)]
        public int AppToolPersonId { get; set; }

        [Required]
        [DefaultValue(0)]
        [DisplayName("Person ID")]
        public int PersonId { get; set; }

        [Required]
        [DisplayName("Type")]
        public int AppToolType { get; set; }   //TODO: add enum for the allowable values?

        [Required]
        [DisplayName("Principal Amount")]
        [DataType(DataType.Currency)]
        [DefaultValue(0.00)]
        public decimal PrincipalAmount { get; set; }

        [Required]
        [DisplayName("Interest Rate")]
        [DefaultValue(0.00)]
        public double InterestRate { get; set; }

        [DisplayName("Number Of Payments")]
        [DefaultValue(120)]
        public int NumberOfPayments { get; set; }

        [DisplayName("Monthly Income")]
        [DataType(DataType.Currency)]
        [DefaultValue(0.00)]
        public decimal MonthlyIncome { get; set; }

        [DisplayName("Number Months Forbearance")]
        [DefaultValue(6)]
        public int NumberOfMonthsInForbearance { get; set; }

        [DisplayName("Forbearance Payment Amount")]
        [DataType(DataType.Currency)]
        [DefaultValue(0.00)]
        public decimal ForbearancePaymentAmount { get; set; }

        [DisplayName("Number Of Forbearance Payments")]
        [DefaultValue(6)]
        public int NumberOfForbearancePayments { get; set; }

        [DisplayName("Balance At Start Deferment")]
        [DataType(DataType.Currency)]
        [DefaultValue(0.00)]
        public decimal BalanceAtStartDeferment { get; set; }

        [DisplayName("Number Of Months In Deferment")]
        [DefaultValue(6)]
        public int NumberOfMonthsInDeferment { get; set; }

    }
}
