using System;

namespace Domain.CommonModels
{
    public class EmpoweredData
    {
        public string PersonName { get; set; }
        public string PersonAddress { get; set; }
        public string Phone { get; set; }
        public string PIN { get; set; }
        //public int SysIdentityCardTypeID { get; set; }
        public string IdentityCard { get; set; }
        public string Email { get; set; }
        public string ReleasedBy { get; set; }
        public DateTime? ReleasedOn { get; set; }
        public int? CountyId { get; set; }
    }

    public class PersonData
    {
        public string PersonName { get; set; }
        public string PIN { get; set; }
        public string IdentityCard { get; set; }
        public string RegistrationNumber { get; set; }
        public string PersonAddress { get; set; }
        public string Phone { get; set; }
        public string CertificateAuthority { get; set; }
        public string CertificateCode { get; set; }
        public DateTime? CertificateDate { get; set; }
        public int SysLegalPersonTypeID { get; set; }
        //public int? SysIdentityCardTypeID { get; set; }
        public bool IsEmpowered { get; set; }
        public bool IsLegalPerson { get; set; }
        public string Email { get; set; }
        public string ReleasedBy { get; set; }
        public DateTime? ReleasedOn { get; set; }
        public int? CountyId { get; set; }
    }

    public class VehicleData
    {
        public int SysVehicleCategoryId { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int ManufactureYear { get; set; }
        public int RegistrationYear { get; set; }
        public int Displacement { get; set; }
        public string RegistrationCertificateNumber { get; set; }
        public string RegistrationCertificateIssuer { get; set; }
        public DateTime? RegistrationCertificateDate { get; set; }
        public string Vin { get; set; }
        public string EngineCode { get; set; }
        public int EnginePower { get; set; }
        public string VehicleIdentificationCard { get; set; }
        public string VehicleIdentificationCardIssuer { get; set; }
        public DateTime? VehicleIdentificationCardDate { get; set; }
        public string LicensePlate { get; set; }
        public int? SysPollutionLevelId { get; set; }
    }

    public class NewVehicleData
    {
        public int? SysVehicleCategoryId { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int? SysPropulsionTypeId { get; set; }
        public int? SysEuroTypeId { get; set; }
        public int? CO2Emission { get; set; }
        public int? SysEcoBonusTypeId { get; set; }
        public int?  EnginePower { get; set; }
        public string EngineCode { get; set; }
        public int? Displacement { get; set; }
        public int? ManufactureYear { get; set; }
        public string VIN { get; set; }
    }

    public class RequestInfoData
    {
        public string DisposalCertificateNumber { get; set; }
        public DateTime? DisposalCertificateDate { get; set; }
        public string DisposalCertificateIssuer { get; set; }
        public string UnlistingCertificateNumber { get; set; }
        public DateTime? UnlistingCertificateDate { get; set; }
        public string UnlistingCertificateIssuer { get; set; }
        public DateTime? ContractDate { get; set; }
        public string ContractNumber { get; set; }

        public string DisposalCertificateIssuerPIN { get; set; }
    }

    public class NewVehicleInvoiceData
    {
        public int ManufactureYear { get; set; }
        public int EnginePower { get; set; }
        public int Displacement { get; set; }
        public string VIN { get; set; }
        public string EngineCode { get; set; }
        public string VehicleIdentificationCard { get; set; }
        public string TemporaryLicensePlate { get; set; }
    }


    public class NewVechicleInvoiceExtendedData : NewVehicleInvoiceData
    {
        public int? SysVehicleCategoryId { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int? SysPropulsionTypeId { get; set; }
        public int? SysEuroTypeId { get; set; }
        public int? CO2Emission { get; set; }
        public int? SysEcoBonusTypeId { get; set; }
    }
    public class InvoiceData
    {
        public string InvoiceSeries { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int SysPaymentTypeId { get; set; }
        public DateTime PaymentDate { get; set; }
        public string InsurancePaymentNumber { get; set; }
        public string InsurerName { get; set; }
        public string LeaseContractNumber { get; set; }
        public DateTime? LeaseContractDate { get; set; }
        public int? LeasePeriod { get; set; }
        public string LeaseCompany { get; set; }
        public string LeaseCompanyCIF { get; set; }

        public DateTime? DeductionDate { get;set;}
        public string DeductionNumber { get; set; }
    }
}
