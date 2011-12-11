Imports OpenTK.Graphics.OpenGL

Public Module modData

    Public SubDirNames As String
    Public SubDirStructures As String
    Public SubDirBrain As String
    Public SubDirBody As String
    Public SubDirPropulsion As String
    Public SubDirBodyPropulsion As String
    Public SubDirConstruction As String
    Public SubDirSensor As String
    Public SubDirRepair As String
    Public SubDirTemplates As String
    Public SubDirWeapons As String
    Public SubDirECM As String
    Public SubDirFeatures As String
    Public SubDirStructurePIE As String
    Public SubDirBodiesPIE As String
    Public SubDirPropPIE As String
    Public SubDirWeaponsPIE As String
    Public SubDirTexpages As String
    Public SubDirAssignWeapons As String
    Public SubDirFeaturePIE As String
    Public SubDirStructureWeapons As String
    Public SubDirPIEs As String

    Public Sub SetDataSubDirs()

        SubDirNames = "messages" & PlatformPathSeparator & "strings" & PlatformPathSeparator & "names.txt"
        SubDirStructures = "stats" & PlatformPathSeparator & "structures.txt"
        SubDirBrain = "stats" & PlatformPathSeparator & "brain.txt"
        SubDirBody = "stats" & PlatformPathSeparator & "body.txt"
        SubDirPropulsion = "stats" & PlatformPathSeparator & "propulsion.txt"
        SubDirBodyPropulsion = "stats" & PlatformPathSeparator & "bodypropulsionimd.txt"
        SubDirConstruction = "stats" & PlatformPathSeparator & "construction.txt"
        SubDirSensor = "stats" & PlatformPathSeparator & "sensor.txt"
        SubDirRepair = "stats" & PlatformPathSeparator & "repair.txt"
        SubDirTemplates = "stats" & PlatformPathSeparator & "templates.txt"
        SubDirWeapons = "stats" & PlatformPathSeparator & "weapons.txt"
        SubDirECM = "stats" & PlatformPathSeparator & "ecm.txt"
        SubDirFeatures = "stats" & PlatformPathSeparator & "features.txt"
        SubDirPIEs = "pies" & PlatformPathSeparator
        'SubDirStructurePIE = "structs" & ospathseperator
        SubDirStructurePIE = SubDirPIEs
        'SubDirBodiesPIE = "components" & ospathseperator & "bodies" & ospathseperator 
        SubDirBodiesPIE = SubDirPIEs
        'SubDirPropPIE = "components" & ospathseperator & "prop" & ospathseperator
        SubDirPropPIE = SubDirPIEs
        'SubDirWeaponsPIE = "components" & ospathseperator & "weapons" & ospathseperator 
        SubDirWeaponsPIE = SubDirPIEs
        SubDirTexpages = "texpages" & PlatformPathSeparator
        SubDirAssignWeapons = "stats" & PlatformPathSeparator & "assignweapons.txt"
        'SubDirFeaturePIE = "features" & ospathseperator 
        SubDirFeaturePIE = SubDirPIEs
        SubDirStructureWeapons = "stats" & PlatformPathSeparator & "structureweapons.txt"
    End Sub

    Public Bodies(-1) As clsBody
    Public BodyCount As Integer

    Public Propulsions(-1) As clsPropulsion
    Public PropulsionCount As Integer

    Public Weapons(-1) As clsWeapon
    Public WeaponCount As Integer

    Public Repairs(-1) As clsRepair
    Public RepairCount As Integer

    Public Constructs(-1) As clsConstruct
    Public ConstructCount As Integer

    Public Sensors(-1) As clsSensor
    Public SensorCount As Integer

    Public Brains(-1) As clsBrain
    Public BrainCount As Integer

    Public ECMs(-1) As clsECM
    Public ECMCount As Integer

    'structure type
    Public Structure sStructure
        Public Code As String
        Public Type As String
        Public Name As String
        Public PIE As String
        Public BasePIE As String
        Public Weapon1 As Integer
        Public Weapon2 As Integer
        Public Weapon3 As Integer
        Public Weapon4 As Integer
        Public ECM As Integer
        Public Sensor As Integer
        Public Footprint As sXY_int
    End Structure
    Public Structure sStructure_List
        Public Structures() As sStructure
        Public StructureCount As Integer
    End Structure

    'component types
    Public Structure sBody
        Public Code As String
        Public Name As String
        Public PIE As String
        Public HitPoints As Integer
        Public Designable As Boolean
    End Structure
    Public Structure sBody_List
        Public Bodies() As sBody
        Public BodyCount As Integer
    End Structure

    Public Structure sPropulsion
        Public Code As String
        Public Name As String
        Public PIE As String
        Public HitPoints As Integer
        Public Designable As Boolean
    End Structure
    Public Structure sPropulsion_List
        Public Propulsions() As sPropulsion
        Public PropulsionCount As Integer
    End Structure

#If Mono267 = 0.0# Then
    Public Structure BodyProp
        Public LeftPIE As String
        Public RightPIE As String
    End Structure
#Else
    Public Class BodyProp
        Public LeftPIE As String
        Public RightPIE As String
    End Class
#End If
    Dim BodyPropulsionPIEs(,) As BodyProp

    Public Structure sWeapon
        Public Code As String
        Public Name As String
        Public PIE As String
        Public PIE2 As String
        Public HitPoints As Integer
        Public Designable As Boolean
    End Structure
    Public Structure sWeapon_List
        Public Weapons() As sWeapon
        Public WeaponCount As Integer
    End Structure

    Public Structure sConstruction
        Public Code As String
        Public Name As String
        Public PIE As String
        Public HitPoints As Integer
        Public Designable As Boolean
    End Structure
    Public Structure sConstruction_List
        Public Constructions() As sConstruction
        Public ConstructionCount As Integer
    End Structure

    Public Structure sSensor
        Public Code As String
        Public Name As String
        Public PIE As String
        Public PIE2 As String
        Public HitPoints As Integer
        Public Designable As Boolean
        Public Location As String
    End Structure
    Public Structure sSensor_List
        Public Sensors() As sSensor
        Public SensorCount As Integer
    End Structure

    Public Structure sRepair
        Public Code As String
        Public Name As String
        Public PIE As String
        Public PIE2 As String
        Public HitPoints As Integer
        Public Designable As Boolean
    End Structure
    Public Structure sRepair_List
        Public Repairs() As sRepair
        Public RepairCount As Integer
    End Structure

    Public Structure sBrain
        Public Code As String
        Public Name As String
        Public Weapon_Num As Integer
        'public PIE As String
        Public HitPoints As Integer
        Public Designable As Boolean
    End Structure
    Public Structure sBrain_List
        Public Brains() As sBrain
        Public BrainCount As Integer
    End Structure

    Public Structure sPIE
        Public Path As String
        Public LCaseFileTitle As String
        Public Model As clsModel
    End Structure
    Public Structure sPIE_List
        Public PIEs() As sPIE
        Public PIECount As Integer
    End Structure

    'template type
    Public Structure sTemplate
        Public Code As String
        Public Name As String
        Public DroidType As String
        Public Body As Integer
        Public Propulsion As Integer
        Public Weapon1 As Integer
        Public Weapon2 As Integer
        Public Weapon3 As Integer
        Public ECM As Integer
        Public Sensor As Integer
        Public Construction As Integer
        Public Repair As Integer
        Public Brain As Integer
    End Structure
    Public Structure sTemplate_List
        Public Templates() As sTemplate
        Public TemplateCount As Integer
    End Structure

    'ecm type
    Public Structure sECM
        Public Code As String
        Public Name As String
        Public PIE As String
        Public HitPoints As Integer
        Public Designable As Boolean
    End Structure
    Public Structure sECM_List
        Public ECMs() As sECM
        Public ECMCount As Integer
    End Structure

    'feature type
    Public Structure sFeature
        Public Code As String
        Public Type As String
        Public Name As String
        Public PIE As String
        Public Footprint As sXY_int
    End Structure
    Public Structure sFeature_List
        Public Features() As sFeature
        Public FeatureCount As Integer
    End Structure

    Public Structure sMod_Data
        Public Structure_List As sStructure_List
        Public Body_List As sBody_List
        Public Propulsion_List As sPropulsion_List
        Public Weapon_List As sWeapon_List
        Public Construction_List As sConstruction_List
        Public Sensor_List As sSensor_List
        Public Repair_List As sRepair_List
        Public Brain_List As sBrain_List
        Public Template_List As sTemplate_List
        Public ECM_List As sECM_List
        Public Feature_List As sFeature_List
    End Structure

    Public Structure sFileData_Entry
        Public FieldValues() As String
        Public FieldCount As Integer
    End Structure
    Public Structure sFileData
        Public Entries() As sFileData_Entry
        Public EntryCount As Integer
    End Structure

    Public Structure sFileData_Entry_Num_List
        Public ResultEntryNum() As Integer
        Public ResultCount As Integer
    End Structure

    Public Sub FileData_Resize(ByRef FileData As sFileData, ByVal NewEntryCount As Integer)

        With FileData
            .EntryCount = NewEntryCount
            ReDim .Entries(.EntryCount - 1)
        End With
    End Sub

    Public Function DataLoad(ByVal Path As String) As clsResult
        Dim ReturnResult As New clsResult

        EndWithPathSeperator(Path)

        'load files

        Dim DataNames As New sFileData
        Dim DataStructures As New sFileData
        Dim DataBrain As New sFileData
        Dim DataBody As New sFileData
        Dim DataPropulsion As New sFileData
        Dim DataBodyPropulsion As New sFileData
        Dim DataConstruction As New sFileData
        Dim DataSensor As New sFileData
        Dim DataRepair As New sFileData
        Dim DataTemplates As New sFileData
        Dim DataECM As New sFileData
        Dim DataFeatures As New sFileData
        Dim DataAssignWeapons As New sFileData
        Dim DataWeapons As New sFileData
        Dim DataStructureWeapons As New sFileData
        Dim Result As sResult

        Result = NamesFileLoad(Path & SubDirNames, DataNames)
        If Not Result.Success Then
            ReturnResult.Problem_Add(Result.Problem)
        End If
        Result = CommaFileLoad(Path & SubDirStructures, DataStructures)
        If Not Result.Success Then
            ReturnResult.Problem_Add(Result.Problem)
        End If
        Result = CommaFileLoad(Path & SubDirBrain, DataBrain)
        If Not Result.Success Then
            ReturnResult.Problem_Add(Result.Problem)
        End If
        Result = CommaFileLoad(Path & SubDirBody, DataBody)
        If Not Result.Success Then
            ReturnResult.Problem_Add(Result.Problem)
        End If
        Result = CommaFileLoad(Path & SubDirPropulsion, DataPropulsion)
        If Not Result.Success Then
            ReturnResult.Problem_Add(Result.Problem)
        End If
        Result = CommaFileLoad(Path & SubDirBodyPropulsion, DataBodyPropulsion)
        If Not Result.Success Then
            ReturnResult.Problem_Add(Result.Problem)
        End If
        Result = CommaFileLoad(Path & SubDirConstruction, DataConstruction)
        If Not Result.Success Then
            ReturnResult.Problem_Add(Result.Problem)
        End If
        Result = CommaFileLoad(Path & SubDirSensor, DataSensor)
        If Not Result.Success Then
            ReturnResult.Problem_Add(Result.Problem)
        End If
        Result = CommaFileLoad(Path & SubDirRepair, DataRepair)
        If Not Result.Success Then
            ReturnResult.Problem_Add(Result.Problem)
        End If
        Result = CommaFileLoad(Path & SubDirTemplates, DataTemplates)
        If Not Result.Success Then
            ReturnResult.Problem_Add(Result.Problem)
        End If
        Result = CommaFileLoad(Path & SubDirECM, DataECM)
        If Not Result.Success Then
            ReturnResult.Problem_Add(Result.Problem)
        End If
        Result = CommaFileLoad(Path & SubDirFeatures, DataFeatures)
        If Not Result.Success Then
            ReturnResult.Problem_Add(Result.Problem)
        End If
        Result = CommaFileLoad(Path & SubDirAssignWeapons, DataAssignWeapons)
        If Not Result.Success Then
            ReturnResult.Problem_Add(Result.Problem)
        End If
        Result = CommaFileLoad(Path & SubDirWeapons, DataWeapons)
        If Not Result.Success Then
            ReturnResult.Problem_Add(Result.Problem)
        End If
        Result = CommaFileLoad(Path & SubDirStructureWeapons, DataStructureWeapons)
        If Not Result.Success Then
            ReturnResult.Problem_Add(Result.Problem)
        End If

        If ReturnResult.HasProblems Then
            Return ReturnResult
        End If

        'validate field amounts and name uniqueness

        Dim Entry_Num_List As sFileData_Entry_Num_List = New sFileData_Entry_Num_List
        'check there are the correct number of fields in names data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataNames, 2, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            ReturnResult.Problem_Add("Invalid entries in names.txt.")
        End If
        'check there are no two names for the same thing
        If Not FileData_Field_Check_Unique(DataNames, 0) Then
            ReturnResult.Problem_Add("There are two entries for the same code in names.txt.")
        End If
        'check there are the correct number of fields in structure data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataStructures, 25, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            ReturnResult.Problem_Add("Entries in structures.txt with wrong number of fields.")
        End If
        'check there are no two names for the same thing
        If Not FileData_Field_Check_Unique(DataStructures, 0) Then
            ReturnResult.Problem_Add("There are two entries for the same code in structures.txt.")
        End If
        'check there are the correct number of fields in brain data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataBrain, 9, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            ReturnResult.Problem_Add("Entries in brain.txt with wrong number of fields.")
        End If
        'check there are no two names for the same thing
        If Not FileData_Field_Check_Unique(DataBrain, 0) Then
            ReturnResult.Problem_Add("There are two entries for the same code in brain.txt.")
        End If
        'check there are the correct number of fields in body data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataBody, 25, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            ReturnResult.Problem_Add("Entries in body.txt with wrong number of fields.")
        End If
        'check there are no two names for the same thing
        If Not FileData_Field_Check_Unique(DataBody, 0) Then
            ReturnResult.Problem_Add("There are two entries for the same code in body.txt.")
        End If
        'check there are the correct number of fields in propulsion data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataPropulsion, 12, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            ReturnResult.Problem_Add("Entries in propulsion.txt with wrong number of fields.")
        End If
        'check there are no two names for the same thing
        If Not FileData_Field_Check_Unique(DataPropulsion, 0) Then
            ReturnResult.Problem_Add("There are two entries for the same code in propulsion.txt.")
        End If
        'check there are the correct number of fields in construction data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataConstruction, 12, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            ReturnResult.Problem_Add("Entries in construction.txt with wrong number of fields.")
        End If
        'check there are no two names for the same thing
        If Not FileData_Field_Check_Unique(DataConstruction, 0) Then
            ReturnResult.Problem_Add("There are two entries for the same code in construction.txt.")
        End If
        'check there are the correct number of fields in sensor data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataSensor, 16, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            ReturnResult.Problem_Add("Entries in sensor.txt with wrong number of fields.")
        End If
        'check there are no two names for the same thing
        If Not FileData_Field_Check_Unique(DataSensor, 0) Then
            ReturnResult.Problem_Add("There are two entries for the same code in sensor.txt.")
        End If
        'check there are the correct number of fields in repair data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataRepair, 14, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            ReturnResult.Problem_Add("Entries in repair.txt with wrong number of fields.")
        End If
        'check there are no two names for the same thing
        If Not FileData_Field_Check_Unique(DataRepair, 0) Then
            ReturnResult.Problem_Add("There are two entries for the same code in repair.txt.")
        End If
        'check there are the correct number of fields in templates data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataTemplates, 12, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            ReturnResult.Problem_Add("Entries in templates.txt with wrong number of fields.")
        End If
        'check there are no two names for the same thing
        If Not FileData_Field_Check_Unique(DataTemplates, 0) Then
            ReturnResult.Problem_Add("There are two entries for the same code in templates.txt.")
        End If
        'check there are the correct number of fields in ecm data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataECM, 14, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            ReturnResult.Problem_Add("Entries in ecm.txt with wrong number of fields.")
        End If
        'check there are no two names for the same thing
        If Not FileData_Field_Check_Unique(DataECM, 0) Then
            ReturnResult.Problem_Add("There are two entries for the same code in ecm.txt.")
        End If
        'check there are the correct number of fields in feature data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataFeatures, 11, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            ReturnResult.Problem_Add("Entries in features.txt with wrong number of fields.")
        End If
        'check there are no two names for the same thing
        If Not FileData_Field_Check_Unique(DataFeatures, 0) Then
            ReturnResult.Problem_Add("There are two entries for the same code in features.txt.")
        End If
        'check there are the correct number of fields in assignweapons data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataAssignWeapons, 5, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            ReturnResult.Problem_Add("Entries in assignweapons.txt with wrong number of fields.")
        End If
        'check there are the correct number of fields in weapon data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataWeapons, 53, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            ReturnResult.Problem_Add("Entries in weapons.txt with wrong number of fields.")
        End If
        'check there are no two names for the same thing
        If Not FileData_Field_Check_Unique(DataWeapons, 0) Then
            ReturnResult.Problem_Add("There are two entries for the same code in features.txt.")
        End If
        'check there are the correct number of fields in bodypropulsion data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataBodyPropulsion, 5, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            ReturnResult.Problem_Add("Entries in bodypropulsionimd.txt with wrong number of fields.")
        End If
        'check there are the correct number of fields in structureweapons data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataStructureWeapons, 6, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            ReturnResult.Problem_Add("Entries in structureweapons.txt with wrong number of fields.")
        End If

        If ReturnResult.HasProblems Then
            Return ReturnResult
        End If

        'interpret data

        Dim NewModData As New sMod_Data

        Dim Structure_Num As Integer
        Dim Brain_Num As Integer
        Dim Body_Num As Integer
        Dim Propulsion_Num As Integer
        Dim Construction_Num As Integer
        Dim Sensor_Num As Integer
        Dim Repair_Num As Integer
        Dim Template_Num As Integer
        Dim ECM_Num As Integer
        Dim Feature_Num As Integer

        With NewModData

            'interpret body
            With .Body_List
                .BodyCount = DataBody.EntryCount
                ReDim .Bodies(.BodyCount - 1)
                For Body_Num = 0 To .BodyCount - 1
                    .Bodies(Body_Num).Code = DataBody.Entries(Body_Num).FieldValues(0)
                    FileData_Entries_Get_From_Field_Value(DataNames, 0, .Bodies(Body_Num).Code, Entry_Num_List)
                    If Entry_Num_List.ResultCount > 0 Then
                        .Bodies(Body_Num).Name = DataNames.Entries(Entry_Num_List.ResultEntryNum(0)).FieldValues(1)
                    Else
                        ReturnResult.Warning_Add("No name in names.txt for body component " & .Bodies(Body_Num).Code & ".")
                    End If
                    InvariantParse_int(DataBody.Entries(Body_Num).FieldValues(6), .Bodies(Body_Num).HitPoints)
                    .Bodies(Body_Num).PIE = LCase(DataBody.Entries(Body_Num).FieldValues(7))
                    .Bodies(Body_Num).Designable = (DataBody.Entries(Body_Num).FieldValues(24) <> "0")
                Next
            End With
            DataBody.EntryCount = 0
            Erase DataBody.Entries

            'interpret propulsion
            With .Propulsion_List
                .PropulsionCount = DataPropulsion.EntryCount
                ReDim .Propulsions(.PropulsionCount - 1)
                For Propulsion_Num = 0 To .PropulsionCount - 1
                    .Propulsions(Propulsion_Num).Code = DataPropulsion.Entries(Propulsion_Num).FieldValues(0)
                    FileData_Entries_Get_From_Field_Value(DataNames, 0, .Propulsions(Propulsion_Num).Code, Entry_Num_List)
                    If Entry_Num_List.ResultCount > 0 Then
                        .Propulsions(Propulsion_Num).Name = DataNames.Entries(Entry_Num_List.ResultEntryNum(0)).FieldValues(1)
                    Else
                        ReturnResult.Warning_Add("No name in names.txt for propulsion component " & .Propulsions(Propulsion_Num).Code & ".")
                    End If
                    InvariantParse_int(DataPropulsion.Entries(Propulsion_Num).FieldValues(7), .Propulsions(Propulsion_Num).HitPoints)
                    .Propulsions(Propulsion_Num).PIE = LCase(DataPropulsion.Entries(Propulsion_Num).FieldValues(8))
                    .Propulsions(Propulsion_Num).Designable = (DataPropulsion.Entries(Propulsion_Num).FieldValues(11) <> "0")
                Next
            End With
            DataPropulsion.EntryCount = 0
            Erase DataPropulsion.Entries

            'interpret construction
            With .Construction_List
                .ConstructionCount = DataConstruction.EntryCount
                ReDim .Constructions(.ConstructionCount - 1)
                For Construction_Num = 0 To .ConstructionCount - 1
                    .Constructions(Construction_Num).Code = DataConstruction.Entries(Construction_Num).FieldValues(0)
                    FileData_Entries_Get_From_Field_Value(DataNames, 0, .Constructions(Construction_Num).Code, Entry_Num_List)
                    If Entry_Num_List.ResultCount > 0 Then
                        .Constructions(Construction_Num).Name = DataNames.Entries(Entry_Num_List.ResultEntryNum(0)).FieldValues(1)
                    Else
                        ReturnResult.Warning_Add("No name in names.txt for construction component " & .Constructions(Construction_Num).Code & ".")
                    End If
                    .Constructions(Construction_Num).PIE = LCase(DataConstruction.Entries(Construction_Num).FieldValues(8))
                    .Constructions(Construction_Num).Designable = (DataConstruction.Entries(Construction_Num).FieldValues(11) <> "0")
                Next
            End With
            DataConstruction.EntryCount = 0
            Erase DataConstruction.Entries

            'interpret weapons
            Dim WeaponNum As Integer
            With .Weapon_List
                .WeaponCount = DataWeapons.EntryCount
                ReDim .Weapons(.WeaponCount - 1)
                For WeaponNum = 0 To .WeaponCount - 1
                    .Weapons(WeaponNum).Code = DataWeapons.Entries(WeaponNum).FieldValues(0)
                    FileData_Entries_Get_From_Field_Value(DataNames, 0, .Weapons(WeaponNum).Code, Entry_Num_List)
                    If Entry_Num_List.ResultCount > 0 Then
                        .Weapons(WeaponNum).Name = DataNames.Entries(Entry_Num_List.ResultEntryNum(0)).FieldValues(1)
                    Else
                        ReturnResult.Warning_Add("No name in names.txt for weapon component " & .Weapons(WeaponNum).Code & ".")
                    End If
                    InvariantParse_int(DataWeapons.Entries(WeaponNum).FieldValues(7), .Weapons(WeaponNum).HitPoints)
                    .Weapons(WeaponNum).PIE = LCase(DataWeapons.Entries(WeaponNum).FieldValues(8))
                    .Weapons(WeaponNum).PIE2 = LCase(DataWeapons.Entries(WeaponNum).FieldValues(9))
                    .Weapons(WeaponNum).Designable = (DataWeapons.Entries(WeaponNum).FieldValues(51) <> "0")
                Next
            End With

            'interpret sensor
            With .Sensor_List
                .SensorCount = DataSensor.EntryCount
                ReDim .Sensors(.SensorCount - 1)
                For Sensor_Num = 0 To .SensorCount - 1
                    .Sensors(Sensor_Num).Code = DataSensor.Entries(Sensor_Num).FieldValues(0)
                    FileData_Entries_Get_From_Field_Value(DataNames, 0, .Sensors(Sensor_Num).Code, Entry_Num_List)
                    If Entry_Num_List.ResultCount > 0 Then
                        .Sensors(Sensor_Num).Name = DataNames.Entries(Entry_Num_List.ResultEntryNum(0)).FieldValues(1)
                    Else
                        ReturnResult.Warning_Add("No name in names.txt for sensor component " & .Sensors(Sensor_Num).Code & ".")
                    End If
                    InvariantParse_int(DataSensor.Entries(Sensor_Num).FieldValues(7), .Sensors(Sensor_Num).HitPoints)
                    .Sensors(Sensor_Num).PIE = LCase(DataSensor.Entries(Sensor_Num).FieldValues(8))
                    .Sensors(Sensor_Num).PIE2 = LCase(DataSensor.Entries(Sensor_Num).FieldValues(9))
                    .Sensors(Sensor_Num).Location = DataSensor.Entries(Sensor_Num).FieldValues(11)
                    .Sensors(Sensor_Num).Designable = (DataSensor.Entries(Sensor_Num).FieldValues(15) <> "0")
                Next
            End With
            DataSensor.EntryCount = 0
            Erase DataSensor.Entries

            'interpret repair
            With .Repair_List
                .RepairCount = DataRepair.EntryCount
                ReDim .Repairs(.RepairCount - 1)
                For Repair_Num = 0 To .RepairCount - 1
                    .Repairs(Repair_Num).Code = DataRepair.Entries(Repair_Num).FieldValues(0)
                    FileData_Entries_Get_From_Field_Value(DataNames, 0, .Repairs(Repair_Num).Code, Entry_Num_List)
                    If Entry_Num_List.ResultCount > 0 Then
                        .Repairs(Repair_Num).Name = DataNames.Entries(Entry_Num_List.ResultEntryNum(0)).FieldValues(1)
                    Else
                        ReturnResult.Warning_Add("No name in names.txt for repair component " & .Repairs(Repair_Num).Code & ".")
                    End If
                    .Repairs(Repair_Num).PIE = LCase(DataRepair.Entries(Repair_Num).FieldValues(9))
                    .Repairs(Repair_Num).PIE2 = LCase(DataRepair.Entries(Repair_Num).FieldValues(10))
                    .Repairs(Repair_Num).Designable = (DataRepair.Entries(Repair_Num).FieldValues(13) <> "0")
                Next
            End With
            DataRepair.EntryCount = 0
            Erase DataRepair.Entries

            'interpret brain
            With .Brain_List
                .BrainCount = DataBrain.EntryCount
                ReDim .Brains(.BrainCount - 1)
                For Brain_Num = 0 To .BrainCount - 1
                    .Brains(Brain_Num).Code = DataBrain.Entries(Brain_Num).FieldValues(0)
                    FileData_Entries_Get_From_Field_Value(DataNames, 0, .Brains(Brain_Num).Code, Entry_Num_List)
                    If Entry_Num_List.ResultCount > 0 Then
                        .Brains(Brain_Num).Name = DataNames.Entries(Entry_Num_List.ResultEntryNum(0)).FieldValues(1)
                    Else
                        ReturnResult.Warning_Add("No name in names.txt for brain component " & .Brains(Brain_Num).Code & ".")
                    End If
                    .Brains(Brain_Num).Weapon_Num = GetWeaponNumFromCode(NewModData, DataBrain.Entries(Brain_Num).FieldValues(7))
                    .Brains(Brain_Num).Designable = True
                Next
            End With
            'we still need brain data

            'interpret ecm
            With .ECM_List
                .ECMCount = DataECM.EntryCount
                ReDim .ECMs(.ECMCount - 1)
                For ECM_Num = 0 To .ECMCount - 1
                    .ECMs(ECM_Num).Code = DataECM.Entries(ECM_Num).FieldValues(0)
                    FileData_Entries_Get_From_Field_Value(DataNames, 0, .ECMs(ECM_Num).Code, Entry_Num_List)
                    If Entry_Num_List.ResultCount > 0 Then
                        .ECMs(ECM_Num).Name = DataNames.Entries(Entry_Num_List.ResultEntryNum(0)).FieldValues(1)
                    Else
                        ReturnResult.Warning_Add("No name in names.txt for ecm component " & .ECMs(ECM_Num).Code & ".")
                    End If
                    InvariantParse_int(DataECM.Entries(ECM_Num).FieldValues(7), .ECMs(ECM_Num).HitPoints)
                    .ECMs(ECM_Num).PIE = LCase(DataECM.Entries(ECM_Num).FieldValues(8))
                    .ECMs(ECM_Num).Designable = False
                Next
            End With

            'interpret feature
            With .Feature_List
                .FeatureCount = DataFeatures.EntryCount
                ReDim .Features(.FeatureCount - 1)
                For Feature_Num = 0 To .FeatureCount - 1
                    .Features(Feature_Num).Code = DataFeatures.Entries(Feature_Num).FieldValues(0)
                    FileData_Entries_Get_From_Field_Value(DataNames, 0, .Features(Feature_Num).Code, Entry_Num_List)
                    If Entry_Num_List.ResultCount > 0 Then
                        .Features(Feature_Num).Name = DataNames.Entries(Entry_Num_List.ResultEntryNum(0)).FieldValues(1)
                    Else
                        ReturnResult.Warning_Add("No name in names.txt for feature " & .Features(Feature_Num).Code & ".")
                    End If
                    .Features(Feature_Num).PIE = LCase(DataFeatures.Entries(Feature_Num).FieldValues(6))
                    If Not InvariantParse_int(DataFeatures.Entries(Feature_Num).FieldValues(1), .Features(Feature_Num).Footprint.X) Then
                        ReturnResult.Warning_Add("Feature footprint-x was not an integer for " & .Features(Feature_Num).Code & ".")
                    End If
                    If Not InvariantParse_int(DataFeatures.Entries(Feature_Num).FieldValues(2), .Features(Feature_Num).Footprint.Y) Then
                        ReturnResult.Warning_Add("Feature footprint-y was not an integer for " & .Features(Feature_Num).Code & ".")
                    End If
                    .Features(Feature_Num).Type = DataFeatures.Entries(Feature_Num).FieldValues(7)
                Next
            End With

            'interpret body-propulsions
            ReDim BodyPropulsionPIEs(.Body_List.BodyCount - 1, .Propulsion_List.PropulsionCount - 1)
            For Body_Num = 0 To .Body_List.BodyCount - 1
                For Propulsion_Num = 0 To .Propulsion_List.PropulsionCount - 1
                    BodyPropulsionPIEs(Body_Num, Propulsion_Num) = New BodyProp
                    BodyPropulsionPIEs(Body_Num, Propulsion_Num).LeftPIE = "0"
                    BodyPropulsionPIEs(Body_Num, Propulsion_Num).RightPIE = "0"
                Next
            Next
            Dim BodyPropNum As Integer
            For BodyPropNum = 0 To DataBodyPropulsion.EntryCount - 1
                With DataBodyPropulsion.Entries(BodyPropNum)
                    Body_Num = GetBodyNumFromCode(NewModData, DataBodyPropulsion.Entries(BodyPropNum).FieldValues(0))
                    Propulsion_Num = GetPropulsionNumFromCode(NewModData, DataBodyPropulsion.Entries(BodyPropNum).FieldValues(1))
                    If Body_Num >= 0 And Propulsion_Num >= 0 Then
                        If DataAssignWeapons.Entries(BodyPropNum).FieldValues(2) <> "0" Then
                            BodyPropulsionPIEs(Body_Num, Propulsion_Num).LeftPIE = LCase(DataBodyPropulsion.Entries(BodyPropNum).FieldValues(2))
                        End If
                        If DataAssignWeapons.Entries(BodyPropNum).FieldValues(3) <> "0" Then
                            BodyPropulsionPIEs(Body_Num, Propulsion_Num).RightPIE = LCase(DataBodyPropulsion.Entries(BodyPropNum).FieldValues(3))
                        End If
                    End If
                End With
            Next

            'interpret templates
            With .Template_List
                .TemplateCount = DataTemplates.EntryCount
                ReDim .Templates(.TemplateCount - 1)
                For Template_Num = 0 To .TemplateCount - 1
                    .Templates(Template_Num).Code = DataTemplates.Entries(Template_Num).FieldValues(0)
                    FileData_Entries_Get_From_Field_Value(DataNames, 0, .Templates(Template_Num).Code, Entry_Num_List)
                    If Entry_Num_List.ResultCount > 0 Then
                        .Templates(Template_Num).Name = DataNames.Entries(Entry_Num_List.ResultEntryNum(0)).FieldValues(1)
                    Else
                        ReturnResult.Warning_Add("No name in names.txt for template component " & .Templates(Template_Num).Code & ".")
                    End If
                    .Templates(Template_Num).Body = GetBodyNumFromCode(NewModData, DataTemplates.Entries(Template_Num).FieldValues(2))
                    .Templates(Template_Num).Propulsion = GetPropulsionNumFromCode(NewModData, DataTemplates.Entries(Template_Num).FieldValues(7))
                    .Templates(Template_Num).Brain = GetBrainNumFromCode(NewModData, DataTemplates.Entries(Template_Num).FieldValues(3))
                    .Templates(Template_Num).Construction = GetConstructionNumFromCode(NewModData, DataTemplates.Entries(Template_Num).FieldValues(4))
                    .Templates(Template_Num).ECM = GetECMNumFromCode(NewModData, DataTemplates.Entries(Template_Num).FieldValues(5))
                    .Templates(Template_Num).Repair = GetRepairNumFromCode(NewModData, DataTemplates.Entries(Template_Num).FieldValues(8))
                    .Templates(Template_Num).DroidType = DataTemplates.Entries(Template_Num).FieldValues(9)
                    .Templates(Template_Num).Sensor = GetSensorNumFromCode(NewModData, DataTemplates.Entries(Template_Num).FieldValues(10))
                    .Templates(Template_Num).Weapon1 = -1
                    .Templates(Template_Num).Weapon2 = -1
                    .Templates(Template_Num).Weapon3 = -1
                Next
            End With

            Dim AssignedWeapon_Num As Integer
            For AssignedWeapon_Num = 0 To DataAssignWeapons.EntryCount - 1
                With DataAssignWeapons.Entries(AssignedWeapon_Num)
                    Template_Num = GetTemplateNumFromCode(NewModData, DataAssignWeapons.Entries(AssignedWeapon_Num).FieldValues(0))
                    If Template_Num >= 0 Then
                        If DataAssignWeapons.Entries(AssignedWeapon_Num).FieldValues(1) <> "NULL" Then
                            NewModData.Template_List.Templates(Template_Num).Weapon1 = GetWeaponNumFromCode(NewModData, DataAssignWeapons.Entries(AssignedWeapon_Num).FieldValues(1))
                        End If
                        If DataAssignWeapons.Entries(AssignedWeapon_Num).FieldValues(2) <> "NULL" Then
                            NewModData.Template_List.Templates(Template_Num).Weapon2 = GetWeaponNumFromCode(NewModData, DataAssignWeapons.Entries(AssignedWeapon_Num).FieldValues(2))
                        End If
                        If DataAssignWeapons.Entries(AssignedWeapon_Num).FieldValues(3) <> "NULL" Then
                            NewModData.Template_List.Templates(Template_Num).Weapon3 = GetWeaponNumFromCode(NewModData, DataAssignWeapons.Entries(AssignedWeapon_Num).FieldValues(3))
                        End If
                    Else
                        'Data_Load.Problem = "Weapons assigned to missing template " & DataAssignWeapons.Entry(AssignedWeapon_Num).FieldValue(0) & "."
                        'Exit Function
                    End If
                End With
            Next

            'interpret structure
            With .Structure_List
                .StructureCount = DataStructures.EntryCount
                ReDim .Structures(.StructureCount - 1)
                For Structure_Num = 0 To .StructureCount - 1
                    .Structures(Structure_Num).Code = DataStructures.Entries(Structure_Num).FieldValues(0)
                    FileData_Entries_Get_From_Field_Value(DataNames, 0, .Structures(Structure_Num).Code, Entry_Num_List)
                    If Entry_Num_List.ResultCount > 0 Then
                        .Structures(Structure_Num).Name = DataNames.Entries(Entry_Num_List.ResultEntryNum(0)).FieldValues(1)
                    Else
                        ReturnResult.Warning_Add("No name in names.txt for structure " & .Structures(Structure_Num).Code & ".")
                    End If
                    .Structures(Structure_Num).Type = DataStructures.Entries(Structure_Num).FieldValues(1)
                    If Not InvariantParse_int(DataStructures.Entries(Structure_Num).FieldValues(5), .Structures(Structure_Num).Footprint.X) Then
                        ReturnResult.Warning_Add("Structure footprint-x was not an integer for " & .Structures(Structure_Num).Code & ".")
                    End If
                    If Not InvariantParse_int(DataStructures.Entries(Structure_Num).FieldValues(6), .Structures(Structure_Num).Footprint.Y) Then
                        ReturnResult.Warning_Add("Structure footprint-y was not an integer for " & .Structures(Structure_Num).Code & ".")
                    End If
                    .Structures(Structure_Num).PIE = LCase(DataStructures.Entries(Structure_Num).FieldValues(21))
                    .Structures(Structure_Num).BasePIE = LCase(DataStructures.Entries(Structure_Num).FieldValues(22))
                    .Structures(Structure_Num).Weapon1 = -1
                    .Structures(Structure_Num).Weapon2 = -1
                    .Structures(Structure_Num).Weapon3 = -1
                    .Structures(Structure_Num).Weapon4 = -1
                    .Structures(Structure_Num).ECM = GetECMNumFromCode(NewModData, DataStructures.Entries(Structure_Num).FieldValues(18))
                    .Structures(Structure_Num).Sensor = GetSensorNumFromCode(NewModData, DataStructures.Entries(Structure_Num).FieldValues(19))
                Next
            End With
            DataStructures.EntryCount = 0
            Erase DataStructures.Entries

            Dim StructureWeaponNum As Integer
            For StructureWeaponNum = 0 To DataStructureWeapons.EntryCount - 1
                With DataStructureWeapons.Entries(StructureWeaponNum)
                    Structure_Num = GetStructureNumFromCode(NewModData, DataStructureWeapons.Entries(StructureWeaponNum).FieldValues(0))
                    If Structure_Num >= 0 Then
                        If DataStructureWeapons.Entries(StructureWeaponNum).FieldValues(1) <> "NULL" Then
                            NewModData.Structure_List.Structures(Structure_Num).Weapon1 = GetWeaponNumFromCode(NewModData, DataStructureWeapons.Entries(StructureWeaponNum).FieldValues(1))
                        End If
                        If DataStructureWeapons.Entries(StructureWeaponNum).FieldValues(2) <> "NULL" Then
                            NewModData.Structure_List.Structures(Structure_Num).Weapon2 = GetWeaponNumFromCode(NewModData, DataStructureWeapons.Entries(StructureWeaponNum).FieldValues(2))
                        End If
                        If DataStructureWeapons.Entries(StructureWeaponNum).FieldValues(3) <> "NULL" Then
                            NewModData.Structure_List.Structures(Structure_Num).Weapon3 = GetWeaponNumFromCode(NewModData, DataStructureWeapons.Entries(StructureWeaponNum).FieldValues(3))
                        End If
                        If DataStructureWeapons.Entries(StructureWeaponNum).FieldValues(4) <> "NULL" Then
                            NewModData.Structure_List.Structures(Structure_Num).Weapon4 = GetWeaponNumFromCode(NewModData, DataStructureWeapons.Entries(StructureWeaponNum).FieldValues(4))
                        End If
                    Else
                        'Data_Load.Problem = "Weapons assigned to missing template " & DataAssignWeapons.Entry(AssignedWeapon_Num).FieldValue(0) & "."
                        'Exit Function
                    End If
                End With
            Next
        End With

        'load texpages

        Dim TexFiles() As String

        Try
            TexFiles = IO.Directory.GetFiles(Path & SubDirTexpages)
        Catch ex As Exception
            ReturnResult.Warning_Add("Unable to access texture pages.")
            ReDim TexFiles(-1)
        End Try

        Dim TexFile_Num As Integer
        Dim tmpString As String
        Dim tmpBitmap As Bitmap = Nothing
        Dim InstrPos2 As Integer
        Dim BitmapTextureArgs As sBitmapGLTexture

        TexturePageCount = 0
        For TexFile_Num = 0 To TexFiles.GetUpperBound(0)
            tmpString = TexFiles(TexFile_Num)
            If Right(tmpString, 4).ToLower = ".png" Then
                If IO.File.Exists(tmpString) Then
                    ReDim Preserve TexturePages(TexturePageCount)
                    Result = LoadBitmap(tmpString, tmpBitmap)
                    If Result.Success Then
                        ReturnResult.AppendAsWarning(BitmapIsGLCompatible(tmpBitmap), "Texture " & ControlChars.Quote & tmpString & ControlChars.Quote & " compatability: ")
                        BitmapTextureArgs.MagFilter = TextureMagFilter.Nearest
                        BitmapTextureArgs.MinFilter = TextureMinFilter.Nearest
                        BitmapTextureArgs.TextureNum = 0
                        BitmapTextureArgs.MipMapLevel = 0
                        BitmapTextureArgs.Texture = tmpBitmap
                        BitmapTextureArgs.Perform()
                        TexturePages(TexturePageCount).GLTexture_Num = BitmapTextureArgs.TextureNum
                    Else
                        ReturnResult.Warning_Add("Unable to load " & tmpString & ": " & Result.Problem)
                    End If
                    InstrPos2 = InStrRev(tmpString, PlatformPathSeparator)
                    TexturePages(TexturePageCount).FileTitle = Strings.Mid(tmpString, InstrPos2 + 1, tmpString.Length - 4 - InstrPos2)
                    TexturePageCount += 1
                Else
                    ReturnResult.Warning_Add("Texture page missing (" & tmpString & ").")
                End If
            End If
        Next

        'create unit objects

        Dim PIE_Files() As String
        Dim PIE_List As sPIE_List

        Dim A As Integer
        Dim C As Integer = 0

        Try
            PIE_Files = IO.Directory.GetFiles(Path & SubDirPIEs)
        Catch ex As Exception
            ReturnResult.Warning_Add("Unable to access PIE files.")
            ReDim PIE_Files(-1)
        End Try

        ReDim PIE_List.PIEs(PIE_Files.GetUpperBound(0))

        Dim SplitPath As sSplitPath

        For A = 0 To PIE_Files.GetUpperBound(0)
            SplitPath = New sSplitPath(PIE_Files(A))
            If SplitPath.FileExtension.ToLower = "pie" Then
                With PIE_List.PIEs(PIE_List.PIECount)
                    .Path = PIE_Files(A)
                    .LCaseFileTitle = SplitPath.FileTitle.ToLower
                End With
                PIE_List.PIECount += 1
            End If
        Next
        ReDim Preserve PIE_List.PIEs(PIE_List.PIECount - 1)

        Dim tmpAttachment As clsUnitType.clsAttachment
        Dim tmpBaseAttachment As clsUnitType.clsAttachment
        Dim tmpConnector As sXYZ_sng
        Dim tmpStructure As clsStructureType
        Dim tmpFeature As clsFeatureType
        Dim tmpTemplate As clsDroidTemplate
        Dim tmpBody As clsBody
        Dim tmpPropulsion As clsPropulsion
        Dim tmpConstruct As clsConstruct
        Dim tmpWeapon As clsWeapon
        Dim tmpRepair As clsRepair
        Dim tmpSensor As clsSensor
        Dim tmpBrain As clsBrain
        Dim tmpECM As clsECM
        Dim B As Integer

        With NewModData

            BodyCount = .Body_List.BodyCount
            ReDim Bodies(BodyCount - 1)
            For A = 0 To BodyCount - 1
                tmpBody = New clsBody
                Bodies(A) = tmpBody
                tmpBody.Num = A
                tmpBody.Code = .Body_List.Bodies(A).Code
                tmpBody.Name = .Body_List.Bodies(A).Name
                tmpBody.Hitpoints = .Body_List.Bodies(A).HitPoints
                tmpBody.Designable = .Body_List.Bodies(A).Designable
                tmpBody.Attachment.Models.Add(GetModelForPIE(PIE_List, .Body_List.Bodies(A).PIE, ReturnResult))
            Next

            PropulsionCount = .Propulsion_List.PropulsionCount
            ReDim Propulsions(PropulsionCount - 1)
            For A = 0 To PropulsionCount - 1
                tmpPropulsion = New clsPropulsion(BodyCount)
                Propulsions(A) = tmpPropulsion
                tmpPropulsion.Code = .Propulsion_List.Propulsions(A).Code
                tmpPropulsion.Name = .Propulsion_List.Propulsions(A).Name
                tmpPropulsion.Hitpoints = .Propulsion_List.Propulsions(A).HitPoints
                tmpPropulsion.Designable = .Propulsion_List.Propulsions(A).Designable
                For B = 0 To BodyCount - 1
                    tmpPropulsion.Bodies(B).LeftAttachment = New clsUnitType.clsAttachment
                    tmpPropulsion.Bodies(B).LeftAttachment.Models.Add(GetModelForPIE(PIE_List, BodyPropulsionPIEs(B, A).LeftPIE, ReturnResult))
                    tmpPropulsion.Bodies(B).RightAttachment = New clsUnitType.clsAttachment
                    tmpPropulsion.Bodies(B).RightAttachment.Models.Add(GetModelForPIE(PIE_List, BodyPropulsionPIEs(B, A).RightPIE, ReturnResult))
                Next
            Next

            ConstructCount = .Construction_List.ConstructionCount
            ReDim Constructs(ConstructCount - 1)
            For A = 0 To ConstructCount - 1
                tmpConstruct = New clsConstruct
                Constructs(A) = tmpConstruct
                tmpConstruct.Code = .Construction_List.Constructions(A).Code
                tmpConstruct.Name = .Construction_List.Constructions(A).Name
                tmpConstruct.HitPoints = .Construction_List.Constructions(A).HitPoints
                tmpConstruct.Designable = .Construction_List.Constructions(A).Designable
                tmpConstruct.Attachment.Models.Add(GetModelForPIE(PIE_List, .Construction_List.Constructions(A).PIE, ReturnResult))
            Next

            WeaponCount = .Weapon_List.WeaponCount
            ReDim Weapons(WeaponCount - 1)
            For A = 0 To WeaponCount - 1
                tmpWeapon = New clsWeapon
                Weapons(A) = tmpWeapon
                tmpWeapon.Code = .Weapon_List.Weapons(A).Code
                tmpWeapon.Name = .Weapon_List.Weapons(A).Name
                tmpWeapon.HitPoints = .Weapon_List.Weapons(A).HitPoints
                tmpWeapon.Designable = .Weapon_List.Weapons(A).Designable
                tmpWeapon.Attachment.Models.Add(GetModelForPIE(PIE_List, .Weapon_List.Weapons(A).PIE, ReturnResult))
                tmpWeapon.Attachment.Models.Add(GetModelForPIE(PIE_List, .Weapon_List.Weapons(A).PIE2, ReturnResult))
            Next

            RepairCount = .Repair_List.RepairCount
            ReDim Repairs(RepairCount - 1)
            For A = 0 To RepairCount - 1
                tmpRepair = New clsRepair
                Repairs(A) = tmpRepair
                tmpRepair.Code = .Repair_List.Repairs(A).Code
                tmpRepair.Name = .Repair_List.Repairs(A).Name
                tmpRepair.HitPoints = .Repair_List.Repairs(A).HitPoints
                tmpRepair.Designable = .Repair_List.Repairs(A).Designable
                tmpRepair.Attachment.Models.Add(GetModelForPIE(PIE_List, .Repair_List.Repairs(A).PIE, ReturnResult))
                tmpRepair.Attachment.Models.Add(GetModelForPIE(PIE_List, .Repair_List.Repairs(A).PIE2, ReturnResult))
            Next

            SensorCount = .Sensor_List.SensorCount
            ReDim Sensors(SensorCount - 1)
            For A = 0 To SensorCount - 1
                tmpSensor = New clsSensor
                Sensors(A) = tmpSensor
                tmpSensor.Code = .Sensor_List.Sensors(A).Code
                tmpSensor.Name = .Sensor_List.Sensors(A).Name
                tmpSensor.HitPoints = .Sensor_List.Sensors(A).HitPoints
                tmpSensor.Designable = .Sensor_List.Sensors(A).Designable
                Select Case .Sensor_List.Sensors(A).Location.ToLower
                    Case "turret"
                        tmpSensor.Location = clsSensor.enumLocation.Turret
                    Case "default"
                        tmpSensor.Location = clsSensor.enumLocation.Invisible
                    Case Else
                        tmpSensor.Location = clsSensor.enumLocation.Invisible
                End Select
                tmpSensor.Attachment.Models.Add(GetModelForPIE(PIE_List, .Sensor_List.Sensors(A).PIE, ReturnResult))
                tmpSensor.Attachment.Models.Add(GetModelForPIE(PIE_List, .Sensor_List.Sensors(A).PIE2, ReturnResult))
            Next

            ECMCount = .ECM_List.ECMCount
            ReDim ECMs(ECMCount - 1)
            For A = 0 To ECMCount - 1
                tmpECM = New clsECM
                ECMs(A) = tmpECM
                tmpECM.Code = .ECM_List.ECMs(A).Code
                tmpECM.Name = .ECM_List.ECMs(A).Name
                tmpECM.HitPoints = .ECM_List.ECMs(A).HitPoints
                tmpECM.Designable = .ECM_List.ECMs(A).Designable
                tmpECM.Attachment.Models.Add(GetModelForPIE(PIE_List, .ECM_List.ECMs(A).PIE, ReturnResult))
            Next

            BrainCount = .Brain_List.BrainCount
            ReDim Brains(BrainCount - 1)
            For A = 0 To BrainCount - 1
                tmpBrain = New clsBrain
                Brains(A) = tmpBrain
                tmpBrain.Code = .Brain_List.Brains(A).Code
                tmpBrain.Name = .Brain_List.Brains(A).Name
                tmpBrain.HitPoints = .Brain_List.Brains(A).HitPoints
                tmpBrain.Designable = .Brain_List.Brains(A).Designable
                B = .Brain_List.Brains(A).Weapon_Num
                If B >= 0 Then
                    tmpBrain.Weapon = Weapons(B)
                    tmpBrain.Attachment.Models.Add(GetModelForPIE(PIE_List, .Weapon_List.Weapons(B).PIE, ReturnResult))
                    tmpBrain.Attachment.Models.Add(GetModelForPIE(PIE_List, .Weapon_List.Weapons(B).PIE2, ReturnResult))
                End If
            Next

            UnitTypeCount = .Structure_List.StructureCount + .Feature_List.FeatureCount + .Template_List.TemplateCount
            ReDim UnitTypes(UnitTypeCount - 1)
            For A = 0 To .Structure_List.StructureCount - 1
                tmpStructure = New clsStructureType
                UnitTypes(C) = tmpStructure
                tmpStructure.StructureNum = A
                tmpStructure.Code = .Structure_List.Structures(A).Code
                tmpStructure.Name = .Structure_List.Structures(A).Name
                tmpStructure.Footprint = .Structure_List.Structures(A).Footprint
                If .Structure_List.Structures(A).Type = "DEMOLISH" Then
                    tmpStructure.StructureType = clsStructureType.enumStructureType.Demolish
                ElseIf .Structure_List.Structures(A).Type = "WALL" Then
                    tmpStructure.StructureType = clsStructureType.enumStructureType.Wall
                ElseIf .Structure_List.Structures(A).Type = "CORNER WALL" Then
                    tmpStructure.StructureType = clsStructureType.enumStructureType.CornerWall
                ElseIf .Structure_List.Structures(A).Type = "FACTORY" Then
                    tmpStructure.StructureType = clsStructureType.enumStructureType.Factory
                ElseIf .Structure_List.Structures(A).Type = "CYBORG FACTORY" Then
                    tmpStructure.StructureType = clsStructureType.enumStructureType.CyborgFactory
                ElseIf .Structure_List.Structures(A).Type = "VTOL FACTORY" Then
                    tmpStructure.StructureType = clsStructureType.enumStructureType.VTOLFactory
                ElseIf .Structure_List.Structures(A).Type = "COMMAND" Then
                    tmpStructure.StructureType = clsStructureType.enumStructureType.Command
                ElseIf .Structure_List.Structures(A).Type = "HQ" Then
                    tmpStructure.StructureType = clsStructureType.enumStructureType.HQ
                ElseIf .Structure_List.Structures(A).Type = "DEFENSE" Then
                    tmpStructure.StructureType = clsStructureType.enumStructureType.Defense
                ElseIf .Structure_List.Structures(A).Type = "POWER GENERATOR" Then
                    tmpStructure.StructureType = clsStructureType.enumStructureType.PowerGenerator
                ElseIf .Structure_List.Structures(A).Type = "POWER MODULE" Then
                    tmpStructure.StructureType = clsStructureType.enumStructureType.PowerModule
                ElseIf .Structure_List.Structures(A).Type = "RESEARCH" Then
                    tmpStructure.StructureType = clsStructureType.enumStructureType.Research
                ElseIf .Structure_List.Structures(A).Type = "RESEARCH MODULE" Then
                    tmpStructure.StructureType = clsStructureType.enumStructureType.ResearchModule
                ElseIf .Structure_List.Structures(A).Type = "FACTORY MODULE" Then
                    tmpStructure.StructureType = clsStructureType.enumStructureType.FactoryModule
                ElseIf .Structure_List.Structures(A).Type = "DOOR" Then
                    tmpStructure.StructureType = clsStructureType.enumStructureType.DOOR
                ElseIf .Structure_List.Structures(A).Type = "REPAIR FACILITY" Then
                    tmpStructure.StructureType = clsStructureType.enumStructureType.RepairFacility
                ElseIf .Structure_List.Structures(A).Type = "SAT UPLINK" Then
                    tmpStructure.StructureType = clsStructureType.enumStructureType.DOOR
                ElseIf .Structure_List.Structures(A).Type = "REARM PAD" Then
                    tmpStructure.StructureType = clsStructureType.enumStructureType.RearmPad
                ElseIf .Structure_List.Structures(A).Type = "MISSILE SILO" Then
                    tmpStructure.StructureType = clsStructureType.enumStructureType.MissileSilo
                ElseIf .Structure_List.Structures(A).Type = "RESOURCE EXTRACTOR" Then
                    tmpStructure.StructureType = clsStructureType.enumStructureType.ResourceExtractor
                Else
                    tmpStructure.StructureType = clsStructureType.enumStructureType.Unknown
                End If

                tmpBaseAttachment = tmpStructure.BaseAttachment
                tmpString = .Structure_List.Structures(A).PIE
                tmpBaseAttachment.Models.Add(GetModelForPIE(PIE_List, tmpString, ReturnResult))
                tmpString = .Structure_List.Structures(A).BasePIE
                tmpStructure.StructureBasePlate = GetModelForPIE(PIE_List, tmpString, ReturnResult)
                If tmpBaseAttachment.Models.ItemCount = 1 Then
                    If tmpBaseAttachment.Models.Item(0).ConnectorCount >= 1 Then
                        tmpConnector = tmpBaseAttachment.Models.Item(0).Connectors(0)
                        If .Structure_List.Structures(A).Weapon1 >= 0 Then
                            If .Weapon_List.Weapons(.Structure_List.Structures(A).Weapon1).Code <> "ZNULLWEAPON" Then
                                tmpString = .Weapon_List.Weapons(.Structure_List.Structures(A).Weapon1).PIE
                                tmpAttachment = tmpBaseAttachment.CreateAttachment()
                                tmpAttachment.Models.Add(GetModelForPIE(PIE_List, tmpString, ReturnResult))
                                tmpAttachment.Pos_Offset = tmpConnector

                                tmpString = .Weapon_List.Weapons(.Structure_List.Structures(A).Weapon1).PIE2
                                tmpAttachment = tmpBaseAttachment.CreateAttachment()
                                tmpAttachment.Models.Add(GetModelForPIE(PIE_List, tmpString, ReturnResult))
                                tmpAttachment.Pos_Offset = tmpConnector
                            End If
                        End If
                        If .Structure_List.Structures(A).ECM >= 0 Then
                            If .ECM_List.ECMs(.Structure_List.Structures(A).ECM).Code <> "ZNULLECM" Then
                                tmpString = .ECM_List.ECMs(.Structure_List.Structures(A).ECM).PIE
                                tmpAttachment = tmpBaseAttachment.CreateAttachment()
                                tmpAttachment.Models.Add(GetModelForPIE(PIE_List, tmpString, ReturnResult))
                                tmpAttachment.Pos_Offset = tmpConnector
                            End If
                        End If
                        If .Structure_List.Structures(A).Sensor >= 0 Then
                            If .Sensor_List.Sensors(.Structure_List.Structures(A).Sensor).Code <> "ZNULLSENSOR" Then
                                tmpString = .Sensor_List.Sensors(.Structure_List.Structures(A).Sensor).PIE
                                tmpAttachment = tmpBaseAttachment.CreateAttachment()
                                tmpAttachment.Models.Add(GetModelForPIE(PIE_List, tmpString, ReturnResult))
                                tmpAttachment.Pos_Offset = tmpConnector

                                tmpString = .Sensor_List.Sensors(.Structure_List.Structures(A).Sensor).PIE2
                                tmpAttachment = tmpBaseAttachment.CreateAttachment()
                                tmpAttachment.Models.Add(GetModelForPIE(PIE_List, tmpString, ReturnResult))
                                tmpAttachment.Pos_Offset = tmpConnector
                            End If
                        End If
                    End If
                End If
                C += 1
            Next
            For A = 0 To .Feature_List.FeatureCount - 1
                tmpFeature = New clsFeatureType
                'tmpFeature.num = C
                UnitTypes(C) = tmpFeature
                tmpFeature.Code = .Feature_List.Features(A).Code
                If .Feature_List.Features(A).Type = "OIL RESOURCE" Then
                    tmpFeature.FeatureType = clsFeatureType.enumFeatureType.OilResource
                End If
                tmpFeature.Name = .Feature_List.Features(A).Name
                tmpFeature.Footprint = .Feature_List.Features(A).Footprint
                tmpFeature.BaseAttachment = New clsUnitType.clsAttachment
                tmpBaseAttachment = tmpFeature.BaseAttachment
                tmpString = .Feature_List.Features(A).PIE
                tmpAttachment = tmpBaseAttachment.CreateAttachment()
                tmpAttachment.Models.Add(GetModelForPIE(PIE_List, tmpString, ReturnResult))
                C += 1
            Next
            Dim TurretConflictCount As Integer = 0
            Dim LoadPartsArgs As clsDroidDesign.sLoadPartsArgs
            For A = 0 To .Template_List.TemplateCount - 1
                tmpTemplate = New clsDroidTemplate
                'tmpTemplate.num = C
                UnitTypes(C) = tmpTemplate
                tmpTemplate.Code = .Template_List.Templates(A).Code
                tmpTemplate.Name = .Template_List.Templates(A).Name
                Select Case .Template_List.Templates(A).DroidType
                    Case "ZNULLDROID"
                        tmpTemplate.TemplateDroidType = TemplateDroidType_Null
                    Case "DROID"
                        tmpTemplate.TemplateDroidType = TemplateDroidType_Droid
                    Case "CYBORG"
                        tmpTemplate.TemplateDroidType = TemplateDroidType_Cyborg
                    Case "CYBORG_CONSTRUCT"
                        tmpTemplate.TemplateDroidType = TemplateDroidType_CyborgConstruct
                    Case "CYBORG_REPAIR"
                        tmpTemplate.TemplateDroidType = TemplateDroidType_CyborgRepair
                    Case "CYBORG_SUPER"
                        tmpTemplate.TemplateDroidType = TemplateDroidType_CyborgSuper
                    Case "TRANSPORTER"
                        tmpTemplate.TemplateDroidType = TemplateDroidType_Transporter
                    Case "PERSON"
                        tmpTemplate.TemplateDroidType = TemplateDroidType_Person
                    Case Else
                        tmpTemplate.TemplateDroidType = Nothing
                        ReturnResult.Warning_Add("Template " & tmpTemplate.GetDisplayText & " had an unrecognised type.")
                End Select
                If .Template_List.Templates(A).Body >= 0 Then
                    LoadPartsArgs.Body = Bodies(.Template_List.Templates(A).Body)
                Else
                    LoadPartsArgs.Body = Nothing
                End If
                If .Template_List.Templates(A).Propulsion >= 0 Then
                    LoadPartsArgs.Propulsion = Propulsions(.Template_List.Templates(A).Propulsion)
                Else
                    LoadPartsArgs.Propulsion = Nothing
                End If
                If .Template_List.Templates(A).Construction >= 0 Then
                    LoadPartsArgs.Construct = Constructs(.Template_List.Templates(A).Construction)
                Else
                    LoadPartsArgs.Construct = Nothing
                End If
                If .Template_List.Templates(A).Repair >= 0 Then
                    LoadPartsArgs.Repair = Repairs(.Template_List.Templates(A).Repair)
                Else
                    LoadPartsArgs.Repair = Nothing
                End If
                If .Template_List.Templates(A).Sensor >= 0 Then
                    LoadPartsArgs.Sensor = Sensors(.Template_List.Templates(A).Sensor)
                Else
                    LoadPartsArgs.Sensor = Nothing
                End If
                If .Template_List.Templates(A).Brain >= 0 Then
                    LoadPartsArgs.Brain = Brains(.Template_List.Templates(A).Brain)
                Else
                    LoadPartsArgs.Brain = Nothing
                End If
                If .Template_List.Templates(A).ECM >= 0 Then
                    LoadPartsArgs.ECM = ECMs(.Template_List.Templates(A).ECM)
                Else
                    LoadPartsArgs.ECM = Nothing
                End If
                If .Template_List.Templates(A).Weapon1 >= 0 Then
                    LoadPartsArgs.Weapon1 = Weapons(.Template_List.Templates(A).Weapon1)
                Else
                    LoadPartsArgs.Weapon1 = Nothing
                End If
                If .Template_List.Templates(A).Weapon2 >= 0 Then
                    LoadPartsArgs.Weapon2 = Weapons(.Template_List.Templates(A).Weapon2)
                Else
                    LoadPartsArgs.Weapon2 = Nothing
                End If
                If .Template_List.Templates(A).Weapon3 >= 0 Then
                    LoadPartsArgs.Weapon3 = Weapons(.Template_List.Templates(A).Weapon3)
                Else
                    LoadPartsArgs.Weapon3 = Nothing
                End If
                If Not tmpTemplate.LoadParts(LoadPartsArgs) Then
                    If TurretConflictCount < 16 Then
                        ReturnResult.Warning_Add("Template " & tmpTemplate.GetDisplayText & " had multiple conflicting turrets.")
                    End If
                    TurretConflictCount += 1
                End If

                C += 1
            Next
            If TurretConflictCount > 0 Then
                ReturnResult.Warning_Add(TurretConflictCount & " templates had multiple conflicting turrets.")
            End If
        End With

        Return ReturnResult
    End Function

    Public Function FileData_Field_Check_Unique(ByRef FileData As sFileData, ByVal Field_Num As Integer) As Boolean
        FileData_Field_Check_Unique = False

        Dim Entry_Num As Integer
        Dim Entry_Num_Other As Integer

        With FileData
            For Entry_Num = 0 To .EntryCount - 1
                For Entry_Num_Other = Entry_Num + 1 To .EntryCount - 1
                    If .Entries(Entry_Num).FieldValues(Field_Num) = .Entries(Entry_Num_Other).FieldValues(Field_Num) Then
                        Return False
                    End If
                Next
            Next
        End With
        Return True
    End Function

    Public Sub FileData_Entries_Get_From_FieldCountNotEqualTo(ByRef FileData As sFileData, ByVal FieldCount As Integer, ByRef Output_Entry_Num_List As sFileData_Entry_Num_List)
        Dim Entry_Num As Integer

        Output_Entry_Num_List.ResultCount = 0
        ReDim Output_Entry_Num_List.ResultEntryNum(FileData.EntryCount - 1)
        With FileData
            For Entry_Num = 0 To .EntryCount - 1
                With .Entries(Entry_Num)
                    If Not .FieldCount = FieldCount Then
                        ReDim Preserve Output_Entry_Num_List.ResultEntryNum(Output_Entry_Num_List.ResultCount)
                        Output_Entry_Num_List.ResultEntryNum(Output_Entry_Num_List.ResultCount) = Entry_Num
                        Output_Entry_Num_List.ResultCount += 1
                    End If
                End With
            Next
        End With
        ReDim Preserve Output_Entry_Num_List.ResultEntryNum(Output_Entry_Num_List.ResultCount - 1)
    End Sub

    Public Sub FileData_Entries_Get_From_Not_FieldCount_InRange(ByRef FileData As sFileData, ByVal FieldCountMin As Integer, ByVal FieldCountMax As Integer, ByRef Output_Entry_Num_List As sFileData_Entry_Num_List)
        Dim Entry_Num As Integer

        Output_Entry_Num_List.ResultCount = 0
        ReDim Output_Entry_Num_List.ResultEntryNum(FileData.EntryCount - 1)
        With FileData
            For Entry_Num = 0 To .EntryCount - 1
                With .Entries(Entry_Num)
                    If (.FieldCount < FieldCountMin Or .FieldCount > FieldCountMax) Then
                        ReDim Preserve Output_Entry_Num_List.ResultEntryNum(Output_Entry_Num_List.ResultCount)
                        Output_Entry_Num_List.ResultEntryNum(Output_Entry_Num_List.ResultCount) = Entry_Num
                        Output_Entry_Num_List.ResultCount += 1
                    End If
                End With
            Next
        End With
        ReDim Preserve Output_Entry_Num_List.ResultEntryNum(Output_Entry_Num_List.ResultCount - 1)
    End Sub

    Public Sub FileData_Entries_Get_From_Field_Value(ByRef FileData As sFileData, ByVal Search_Field_Num As Integer, ByVal Search_String As String, ByRef Output_Entry_Num_List As sFileData_Entry_Num_List)
        Dim Entry_Num As Integer

        Output_Entry_Num_List.ResultCount = 0
        ReDim Output_Entry_Num_List.ResultEntryNum(FileData.EntryCount - 1)
        With FileData
            For Entry_Num = 0 To .EntryCount - 1
                With .Entries(Entry_Num)
                    If .FieldValues(Search_Field_Num) = Search_String Then
                        Output_Entry_Num_List.ResultEntryNum(Output_Entry_Num_List.ResultCount) = Entry_Num
                        Output_Entry_Num_List.ResultCount += 1
                    End If
                End With
            Next
        End With
        ReDim Preserve Output_Entry_Num_List.ResultEntryNum(Output_Entry_Num_List.ResultCount - 1)
    End Sub

    Public Structure sBytes
        Public Bytes() As Byte
    End Structure
    Public Structure sLines
        Public Lines() As String

        Public Sub RemoveComments()
            Dim LineNum As Integer
            Dim LineCount As Integer = Lines.GetUpperBound(0) + 1
            Dim InCommentBlock As Boolean
            Dim CommentStart As Integer
            Dim CharNum As Integer
            Dim CommentLength As Integer

            For LineNum = 0 To LineCount - 1
                CharNum = 0
                If InCommentBlock Then
                    CommentStart = 0
                End If
                Do
                    If CharNum >= Lines(LineNum).Length Then
                        If InCommentBlock Then
                            Lines(LineNum) = Strings.Left(Lines(LineNum), CommentStart)
                        End If
                        Exit Do
                    ElseIf InCommentBlock Then
                        If Lines(LineNum).Chars(CharNum) = "*"c Then
                            CharNum += 1
                            If CharNum >= Lines(LineNum).Length Then

                            ElseIf Lines(LineNum).Chars(CharNum) = "/"c Then
                                CharNum += 1
                                CommentLength = CharNum - CommentStart
                                InCommentBlock = False
                                Lines(LineNum) = Strings.Left(Lines(LineNum), CommentStart) & Strings.Right(Lines(LineNum), Lines(LineNum).Length - (CommentStart + CommentLength))
                                CharNum -= CommentLength
                            End If
                        Else
                            CharNum += 1
                        End If
                    ElseIf Lines(LineNum).Chars(CharNum) = "/"c Then
                        CharNum += 1
                        If CharNum >= Lines(LineNum).Length Then

                        ElseIf Lines(LineNum).Chars(CharNum) = "/"c Then
                            CommentStart = CharNum - 1
                            CharNum = Lines(LineNum).Length
                            CommentLength = CharNum - CommentStart
                            Lines(LineNum) = Strings.Left(Lines(LineNum), CommentStart) & Strings.Right(Lines(LineNum), Lines(LineNum).Length - (CommentStart + CommentLength))
                            Exit Do
                        ElseIf Lines(LineNum).Chars(CharNum) = "*"c Then
                            CommentStart = CharNum - 1
                            CharNum += 1
                            InCommentBlock = True
                        End If
                    Else
                        CharNum += 1
                    End If
                Loop
            Next
        End Sub
    End Structure

    Public Sub BytesToLines(ByRef Bytes As sBytes, ByRef OutputLines As sLines)
        Dim ByteCount As Integer = Bytes.Bytes.GetUpperBound(0) + 1
        Dim StartNum As Integer
        Dim Length As Integer
        Dim ByteNum As Integer
        Dim LineCount As Integer
        Dim CharByteNum As Integer
        Dim Chars(511) As Char
        Dim Count As Integer

        If OutputLines.Lines Is Nothing Then
            ReDim OutputLines.Lines(-1)
            LineCount = 0
        Else
            LineCount = OutputLines.Lines.GetUpperBound(0) + 1
        End If

        StartNum = 0
        Do While StartNum < ByteCount
            ByteNum = StartNum
            Do
                If ByteNum >= ByteCount Then
                    Length = ByteNum - StartNum
                    Exit Do
                ElseIf Bytes.Bytes(ByteNum) = 13 Then
                    Length = ByteNum - StartNum
                    ByteNum += 1
                    If ByteNum >= ByteCount Then

                    ElseIf Bytes.Bytes(ByteNum) = 10 Then
                        ByteNum += 1
                    End If
                    Exit Do
                ElseIf Bytes.Bytes(ByteNum) = 10 Then
                    Length = ByteNum - StartNum
                    ByteNum += 1
                    Exit Do
                Else
                    ByteNum += 1
                End If
            Loop

            If OutputLines.Lines.GetUpperBound(0) < LineCount Then
                ReDim Preserve OutputLines.Lines(LineCount + 127)
            End If
            If Length > 0 Then
                If Chars.GetUpperBound(0) < Length - 1 Then
                    ReDim Chars(Length - 1)
                End If
                Count = 0
                For CharByteNum = StartNum To StartNum + Length - 1
                    Chars(Count) = Chr(Bytes.Bytes(CharByteNum))
                    Count += 1
                Next
                OutputLines.Lines(LineCount) = New String(Chars, 0, Length)
            Else
                OutputLines.Lines(LineCount) = ""
            End If
            LineCount += 1

            StartNum = ByteNum
        Loop
        ReDim Preserve OutputLines.Lines(LineCount - 1)
    End Sub

    Public Function CommaFileLoad(ByVal Path As String, ByRef FileData As sFileData) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Success = False
        ReturnResult.Problem = ""

        Dim Bytes As sBytes
        Dim LineData As New sLines
        Dim LineCount As Integer
        Dim LineNum As Integer
        Dim strTemp As String
        Dim CommaPos As Integer
        Dim Flag As Boolean

        'load all bytes
        Try
            Bytes.Bytes = IO.File.ReadAllBytes(Path)
        Catch ex As Exception
            ReturnResult.Problem = ex.Message
            Return ReturnResult
        End Try

        BytesToLines(Bytes, LineData)

        LineData.RemoveComments()
        LineCount = LineData.Lines.GetUpperBound(0) + 1

        With FileData
            ReDim .Entries(LineCount - 1)
            For LineNum = 0 To LineCount - 1
                strTemp = LineData.Lines(LineNum)
                Flag = False
                With .Entries(.EntryCount)
                    ReDim .FieldValues(63)
                    Do
                        If .FieldValues.GetUpperBound(0) < .FieldCount Then
                            ReDim Preserve .FieldValues(.FieldCount + 16)
                        End If
                        CommaPos = InStr(1, strTemp, ",")
                        If CommaPos = 0 Then
                            .FieldValues(.FieldCount) = strTemp
                            Flag = True
                        Else
                            .FieldValues(.FieldCount) = Left(strTemp, CommaPos - 1)
                            strTemp = Right(strTemp, strTemp.Length - CommaPos)
                        End If
                        .FieldCount += 1
                        If Flag Then
                            ReDim Preserve .FieldValues(.FieldCount - 1)
                            Exit Do
                        End If
                    Loop
                End With
                .EntryCount += 1
            Next
            ReDim Preserve .Entries(.EntryCount - 1)
        End With

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    Public Function NamesFileLoad(ByVal Path As String, ByRef FileData As sFileData) As sResult
        Dim ReturnResult As sResult
        ReturnResult.Problem = ""
        ReturnResult.Success = False

        Dim Bytes As sBytes
        Dim LineData As New sLines
        Dim LineCount As Integer
        Dim LineNum As Integer
        Dim strTemp As String
        Dim A As Integer
        Dim B As Integer
        Dim tmpChar As Char

        'load all bytes
        Try
            Bytes.Bytes = IO.File.ReadAllBytes(Path)
        Catch ex As Exception
            ReturnResult.Problem = ex.Message
            Return ReturnResult
        End Try

        BytesToLines(Bytes, LineData)

        LineData.RemoveComments()
        LineCount = LineData.Lines.GetUpperBound(0) + 1

        'output as entries with fields
        Dim Code As String
        With FileData
            ReDim .Entries(LineCount - 1)
            For LineNum = 0 To LineCount - 1
                strTemp = LineData.Lines(LineNum)
                'get code until space or tab
                For A = 0 To strTemp.Length - 1
                    tmpChar = strTemp.Chars(A)
                    If tmpChar = " "c Or Asc(tmpChar) = 9 Then
                        Exit For
                    End If
                Next
                Code = Left(strTemp, A)
                If Code <> "" Then
                    With .Entries(.EntryCount)
                        .FieldCount = 2
                        ReDim Preserve .FieldValues(.FieldCount - 1)
                        .FieldValues(0) = Code
                        'ignore everything until the quotation mark
                        For B = A + 1 To strTemp.Length - 1
                            tmpChar = strTemp.Chars(B)
                            If Asc(tmpChar) = 34 Then
                                Exit For
                            End If
                        Next
                        'get name until second quotation mark
                        For A = B + 1 To strTemp.Length - 1
                            tmpChar = strTemp.Chars(A)
                            If Asc(tmpChar) = 34 Then
                                Exit For
                            End If
                        Next
                        If B + 1 < strTemp.Length Then
                            .FieldValues(1) = strTemp.Substring(B + 1, A - (B + 1))
                        Else
                            .FieldValues(1) = ""
                        End If
                    End With
                    .EntryCount += 1
                End If
            Next
            ReDim Preserve .Entries(.EntryCount - 1)
        End With

        ReturnResult.Success = True
        Return ReturnResult
    End Function

    Public Function GetConstructionNumFromCode(ByRef Mod_Data As sMod_Data, ByVal Code As String) As Integer
        Dim Construction_Num As Integer

        With Mod_Data.Construction_List
            For Construction_Num = 0 To .ConstructionCount - 1
                If .Constructions(Construction_Num).Code = Code Then
                    Return Construction_Num
                End If
            Next
        End With
        Return -1
    End Function

    Public Function GetBodyNumFromCode(ByRef Mod_Data As sMod_Data, ByVal Code As String) As Integer
        Dim Body_Num As Integer

        With Mod_Data.Body_List
            For Body_Num = 0 To .BodyCount - 1
                If .Bodies(Body_Num).Code = Code Then
                    Return Body_Num
                End If
            Next
        End With
        Return -1
    End Function

    Public Function GetBrainNumFromCode(ByRef Mod_Data As sMod_Data, ByVal Code As String) As Integer
        Dim Brain_Num As Integer

        With Mod_Data.Brain_List
            For Brain_Num = 0 To .BrainCount - 1
                If .Brains(Brain_Num).Code = Code Then
                    Return Brain_Num
                End If
            Next
        End With
        Return -1
    End Function

    Public Function GetPropulsionNumFromCode(ByRef Mod_Data As sMod_Data, ByVal Code As String) As Integer
        Dim Propulsion_Num As Integer

        With Mod_Data.Propulsion_List
            For Propulsion_Num = 0 To .PropulsionCount - 1
                If .Propulsions(Propulsion_Num).Code = Code Then
                    Return Propulsion_Num
                End If
            Next
        End With
        Return -1
    End Function

    Public Function GetRepairNumFromCode(ByRef Mod_Data As sMod_Data, ByVal Code As String) As Integer
        Dim Repair_Num As Integer

        With Mod_Data.Repair_List
            For Repair_Num = 0 To .RepairCount - 1
                If .Repairs(Repair_Num).Code = Code Then
                    Return Repair_Num
                End If
            Next
        End With
        Return -1
    End Function

    Public Function GetECMNumFromCode(ByRef Mod_Data As sMod_Data, ByVal Code As String) As Integer
        Dim ECM_Num As Integer

        With Mod_Data.ECM_List
            For ECM_Num = 0 To .ECMCount - 1
                If .ECMs(ECM_Num).Code = Code Then
                    Return ECM_Num
                End If
            Next
        End With
        Return -1
    End Function

    Public Function GetSensorNumFromCode(ByRef Mod_Data As sMod_Data, ByVal Code As String) As Integer
        Dim Sensor_Num As Integer

        With Mod_Data.Sensor_List
            For Sensor_Num = 0 To .SensorCount - 1
                If .Sensors(Sensor_Num).Code = Code Then
                    Return Sensor_Num
                End If
            Next
        End With
        Return -1
    End Function

    Public Function GetWeaponNumFromCode(ByRef Mod_Data As sMod_Data, ByVal Code As String) As Integer
        Dim Weapon_Num As Integer

        With Mod_Data.Weapon_List
            For Weapon_Num = 0 To .WeaponCount - 1
                If .Weapons(Weapon_Num).Code = Code Then
                    Return Weapon_Num
                End If
            Next
        End With
        Return -1
    End Function

    Public Function GetTemplateNumFromCode(ByRef Mod_Data As sMod_Data, ByVal Code As String) As Integer
        Dim Template_Num As Integer

        With Mod_Data.Template_List
            For Template_Num = 0 To .TemplateCount - 1
                If .Templates(Template_Num).Code = Code Then
                    Return Template_Num
                End If
            Next
        End With
        Return -1
    End Function

    Public Function GetStructureNumFromCode(ByRef Mod_Data As sMod_Data, ByVal Code As String) As Integer
        Dim Structure_Num As Integer

        With Mod_Data.Structure_List
            For Structure_Num = 0 To .StructureCount - 1
                If .Structures(Structure_Num).Code = Code Then
                    Return Structure_Num
                End If
            Next
        End With
        Return -1
    End Function

    Public Function GetModelForPIE(ByRef PIE_List As sPIE_List, ByVal PIE_LCaseFileTitle As String, ByVal ResultOutput As clsResult) As clsModel

        If PIE_LCaseFileTitle = "0" Then
            Return Nothing
        End If

        Dim A As Integer
        Dim PIEFile As IO.StreamReader

        For A = 0 To PIE_List.PIECount - 1
            If PIE_List.PIEs(A).LCaseFileTitle = PIE_LCaseFileTitle Then
                If PIE_List.PIEs(A).Model Is Nothing Then
                    PIE_List.PIEs(A).Model = New clsModel
                    Try
                        PIEFile = New IO.StreamReader(PIE_List.PIEs(A).Path)
                        Try
                            ResultOutput.AppendAsWarning(PIE_List.PIEs(A).Model.LoadPIE(PIEFile), "Loading PIE " & ControlChars.Quote & PIE_LCaseFileTitle & ControlChars.Quote & ": ")
                        Catch ex As Exception
                            PIEFile.Close()
                            ResultOutput.Warning_Add(PIE_LCaseFileTitle & " produced error " & ex.Message & ".")
                            Return PIE_List.PIEs(A).Model
                        End Try
                    Catch ex As Exception
                        ResultOutput.Warning_Add("Unable to open PIE " & ControlChars.Quote & PIE_List.PIEs(A).Path & ControlChars.Quote & ": " & ex.Message)
                    End Try
                End If
                Return PIE_List.PIEs(A).Model
            End If
        Next
        ResultOutput.Warning_Add("Unable to find PIE file " & PIE_LCaseFileTitle)
        Return Nothing
    End Function

    Public Function FindBodyCode(ByVal Code As String) As clsBody
        Dim A As Integer

        For A = 0 To BodyCount - 1
            If Bodies(A).Code = Code Then
                Return Bodies(A)
            End If
        Next
        Return Nothing
    End Function

    Public Function FindPropulsionCode(ByVal Code As String) As clsPropulsion
        Dim A As Integer

        For A = 0 To PropulsionCount - 1
            If Propulsions(A).Code = Code Then
                Return Propulsions(A)
            End If
        Next
        Return Nothing
    End Function

    Public Function FindRepairCode(ByVal Code As String) As clsRepair
        Dim A As Integer

        For A = 0 To RepairCount - 1
            If Repairs(A).Code = Code Then
                Return Repairs(A)
            End If
        Next
        Return Nothing
    End Function

    Public Function FindConstructCode(ByVal Code As String) As clsConstruct
        Dim A As Integer

        For A = 0 To ConstructCount - 1
            If Constructs(A).Code = Code Then
                Return Constructs(A)
            End If
        Next
        Return Nothing
    End Function

    Public Function FindSensorCode(ByVal Code As String) As clsSensor
        Dim A As Integer

        For A = 0 To SensorCount - 1
            If Sensors(A).Code = Code Then
                Return Sensors(A)
            End If
        Next
        Return Nothing
    End Function

    Public Function FindWeaponCode(ByVal Code As String) As clsWeapon
        Dim A As Integer

        For A = 0 To WeaponCount - 1
            If Weapons(A).Code = Code Then
                Return Weapons(A)
            End If
        Next
        Return Nothing
    End Function

    Public Function FindBrainCode(ByVal Code As String) As clsBrain
        Dim A As Integer

        For A = 0 To BrainCount - 1
            If Brains(A).Code = Code Then
                Return Brains(A)
            End If
        Next
        Return Nothing
    End Function

    Public Function FindECMCode(ByVal Code As String) As clsECM
        Dim A As Integer

        For A = 0 To ECMCount - 1
            If ECMs(A).Code = Code Then
                Return ECMs(A)
            End If
        Next
        Return Nothing
    End Function

    Public Function FindTurretCode(ByVal SaveType As Byte, ByVal Code As String) As clsTurret

        Select Case SaveType
            Case 0
                FindTurretCode = FindWeaponCode(Code)
            Case 1
                FindTurretCode = FindConstructCode(Code)
            Case 2
                FindTurretCode = FindRepairCode(Code)
            Case 3
                FindTurretCode = FindSensorCode(Code)
            Case 4
                FindTurretCode = FindBrainCode(Code)
            Case 5
                FindTurretCode = FindECMCode(Code)
            Case Else
                Return Nothing
        End Select
    End Function
End Module