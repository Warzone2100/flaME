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
    'public SubDirStructurePIE As String
    Public SubDirStructurePIE As String
    'public SubDirBodiesPIE As String
    Public SubDirBodiesPIE As String
    'public SubDirPropPIE As String
    Public SubDirPropPIE As String
    'public SubDirWeaponsPIE As String
    Public SubDirWeaponsPIE As String
    Public SubDirTexpages As String
    Public SubDirAssignWeapons As String
    'public SubDirFeaturePIE As String
    Public SubDirFeaturePIE As String
    Public SubDirStructureWeapons As String
    Public SubDirPIEs As String

    Public Sub SetDataSubDirs()

        SubDirNames = "messages" & OSPathSeperator & "strings" & OSPathSeperator & "names.txt"
        SubDirStructures = "stats" & OSPathSeperator & "structures.txt"
        SubDirBrain = "stats" & OSPathSeperator & "brain.txt"
        SubDirBody = "stats" & ospathseperator & "body.txt"
        SubDirPropulsion = "stats" & ospathseperator & "propulsion.txt"
        SubDirBodyPropulsion = "stats" & ospathseperator & "bodypropulsionimd.txt"
        SubDirConstruction = "stats" & ospathseperator & "construction.txt"
        SubDirSensor = "stats" & ospathseperator & "sensor.txt"
        SubDirRepair = "stats" & ospathseperator & "repair.txt"
        SubDirTemplates = "stats" & ospathseperator & "templates.txt"
        SubDirWeapons = "stats" & ospathseperator & "weapons.txt"
        SubDirECM = "stats" & ospathseperator & "ecm.txt"
        SubDirFeatures = "stats" & OSPathSeperator & "features.txt"
        SubDirPIEs = "pies" & OSPathSeperator
        'SubDirStructurePIE = "structs" & ospathseperator
        SubDirStructurePIE = SubDirPIEs
        'SubDirBodiesPIE = "components" & ospathseperator & "bodies" & ospathseperator 
        SubDirBodiesPIE = SubDirPIEs
        'SubDirPropPIE = "components" & ospathseperator & "prop" & ospathseperator
        SubDirPropPIE = SubDirPIEs
        'SubDirWeaponsPIE = "components" & ospathseperator & "weapons" & ospathseperator 
        SubDirWeaponsPIE = SubDirPIEs
        SubdirTexpages = "texpages" & ospathseperator
        SubDirAssignWeapons = "stats" & ospathseperator & "assignweapons.txt"
        'SubDirFeaturePIE = "features" & ospathseperator 
        SubDirFeaturePIE = SubDirPIEs
        SubDirStructureWeapons = "stats" & ospathseperator & "structureweapons.txt"
    End Sub

    Public Enum enumPIE_Purpose As Byte
        Base
        Propulsion
        Component
    End Enum

    'structure type
    Structure sStructure
        Dim Code As String
        Dim Type As String
        Dim Name As String
        Dim PIE As String
        Dim BasePIE As String
        Dim Weapon1 As Integer
        Dim Weapon2 As Integer
        Dim Weapon3 As Integer
        Dim Weapon4 As Integer
        Dim ECM As Integer
        Dim Sensor As Integer
        Dim Footprint As sXY_int
    End Structure
    Structure sStructure_List
        Dim Structures() As sStructure
        Dim StructureCount As Integer
    End Structure

    'component types
    Structure sBody
        Dim Code As String
        Dim Name As String
        Dim PIE As String
    End Structure
    Structure sBody_List
        Dim Bodies() As sBody
        Dim BodyCount As Integer
    End Structure

    Structure sPropulsion
        Dim Code As String
        Dim Name As String
        Dim PIE As String
    End Structure
    Structure sPropulsion_List
        Dim Propulsions() As sPropulsion
        Dim PropulsionCount As Integer
    End Structure

    Structure BodyProp
        Dim LeftPIE As String
        Dim RightPIE As String
    End Structure
    Dim BodyPropulsions(,) As BodyProp

    Structure sWeapon
        Dim Code As String
        Dim Name As String
        Dim PIE As String
        Dim PIE2 As String
    End Structure
    Structure sWeapon_List
        Dim Weapons() As sWeapon
        Dim WeaponCount As Integer
    End Structure

    Structure sConstruction
        Dim Code As String
        Dim Name As String
        Dim PIE As String
    End Structure
    Structure sConstruction_List
        Dim Constructions() As sConstruction
        Dim ConstructionCount As Integer
    End Structure

    Structure sSensor
        Dim Code As String
        Dim Name As String
        Dim PIE As String
        Dim PIE2 As String
    End Structure
    Structure sSensor_List
        Dim Sensors() As sSensor
        Dim SensorCount As Integer
    End Structure

    Structure sRepair
        Dim Code As String
        Dim Name As String
        Dim PIE As String
    End Structure
    Structure sRepair_List
        Dim Repairs() As sRepair
        Dim RepairCount As Integer
    End Structure

    Structure sBrain
        Dim Code As String
        Dim Name As String
        Dim Weapon_Num As Integer
        Dim PIE As String
    End Structure
    Structure sBrain_List
        Dim Brains() As sBrain
        Dim BrainCount As Integer
    End Structure

    Structure sPIE
        Dim Path As String
        Dim LCaseFileTitle As String
        Dim Model As clsModel
    End Structure
    Structure sPIE_List
        Dim PIEs() As sPIE
        Dim PIECount As Integer
    End Structure

    Const Component_Type_Body As Integer = 0
    Const Component_Type_Propulsion As Integer = 1
    Const Component_Type_Weapon As Integer = 2
    Const Component_Type_Construction As Integer = 3
    Const Component_Type_Sensor As Integer = 4
    Const Component_Type_Repair As Integer = 5
    Const Component_Type_Brain As Integer = 6

    'template type
    Structure sTemplate
        Dim Code As String
        Dim Name As String
        Dim Body As Integer
        Dim Propulsion As Integer
        Dim Weapon1 As Integer
        Dim Weapon2 As Integer
        Dim Weapon3 As Integer
        Dim ECM As Integer
        Dim Sensor As Integer
        Dim Construction As Integer
        Dim Repair As Integer
        Dim Brain As Integer
    End Structure
    Structure sTemplate_List
        Dim Templates() As sTemplate
        Dim TemplateCount As Integer
    End Structure

    'ecm type
    Structure sECM
        Dim Code As String
        Dim Name As String
        Dim PIE As String
    End Structure
    Structure sECM_List
        Dim ECMs() As sECM
        Dim ECMCount As Integer
    End Structure

    'feature type
    Structure sFeature
        Dim Code As String
        Dim Type As String
        Dim Name As String
        Dim PIE As String
        Dim Footprint As sXY_int
    End Structure
    Structure sFeature_List
        Dim Features() As sFeature
        Dim FeatureCount As Integer
    End Structure

    Structure sMod_Data
        Dim Structure_List As sStructure_List
        Dim Body_List As sBody_List
        Dim Propulsion_List As sPropulsion_List
        Dim Weapon_List As sWeapon_List
        Dim Construction_List As sConstruction_List
        Dim Sensor_List As sSensor_List
        Dim Repair_List As sRepair_List
        Dim Brain_List As sBrain_List
        Dim Template_List As sTemplate_List
        Dim ECM_List As sECM_List
        Dim Feature_List As sFeature_List
    End Structure
    Public Mod_Data_Main As sMod_Data

    Structure sFileData_Entry
        Dim FieldValue() As String
        Dim FieldCount As Integer
    End Structure
    Structure sFileData
        Dim Entry() As sFileData_Entry
        Dim EntryCount As Integer
    End Structure

    Structure sFileData_Entry_Num_List
        Dim Result_Entry_Num() As Integer
        Dim ResultCount As Integer
    End Structure

    Sub FileData_Resize(ByRef FileData As sFileData, ByVal NewEntryCount As Integer)

        With FileData
            .EntryCount = NewEntryCount
            ReDim .Entry(.EntryCount - 1)
        End With
    End Sub

    Function Data_Load(ByVal Path As String) As sResult
        Data_Load.Success = False
        Data_Load.Problem = ""

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

        Result = Names_File_Load(Path & SubDirNames, DataNames)
        If Not Result.Success Then
            Data_Load.Problem = Result.Problem
            Exit Function
        End If
        Result = Comma_File_Load(Path & SubDirStructures, DataStructures)
        If Not Result.Success Then
            Data_Load.Problem = Result.Problem
            Exit Function
        End If
        Result = Comma_File_Load(Path & SubDirBrain, DataBrain)
        If Not Result.Success Then
            Data_Load.Problem = Result.Problem
            Exit Function
        End If
        Result = Comma_File_Load(Path & SubDirBody, DataBody)
        If Not Result.Success Then
            Data_Load.Problem = Result.Problem
            Exit Function
        End If
        Result = Comma_File_Load(Path & SubDirPropulsion, DataPropulsion)
        If Not Result.Success Then
            Data_Load.Problem = Result.Problem
            Exit Function
        End If
        Result = Comma_File_Load(Path & SubDirBodyPropulsion, DataBodyPropulsion)
        If Not Result.Success Then
            Data_Load.Problem = Result.Problem
            Exit Function
        End If
        Result = Comma_File_Load(Path & SubDirConstruction, DataConstruction)
        If Not Result.Success Then
            Data_Load.Problem = Result.Problem
            Exit Function
        End If
        Result = Comma_File_Load(Path & SubDirSensor, DataSensor)
        If Not Result.Success Then
            Data_Load.Problem = Result.Problem
            Exit Function
        End If
        Result = Comma_File_Load(Path & SubDirRepair, DataRepair)
        If Not Result.Success Then
            Data_Load.Problem = Result.Problem
            Exit Function
        End If
        Result = Comma_File_Load(Path & SubDirTemplates, DataTemplates)
        If Not Result.Success Then
            Data_Load.Problem = Result.Problem
            Exit Function
        End If
        Result = Comma_File_Load(Path & SubDirECM, DataECM)
        If Not Result.Success Then
            Data_Load.Problem = Result.Problem
            Exit Function
        End If
        Result = Comma_File_Load(Path & SubDirFeatures, DataFeatures)
        If Not Result.Success Then
            Data_Load.Problem = Result.Problem
            Exit Function
        End If
        Result = Comma_File_Load(Path & SubDirAssignWeapons, DataAssignWeapons)
        If Not Result.Success Then
            Data_Load.Problem = Result.Problem
            Exit Function
        End If
        Result = Comma_File_Load(Path & SubDirWeapons, DataWeapons)
        If Not Result.Success Then
            Data_Load.Problem = Result.Problem
            Exit Function
        End If
        Result = Comma_File_Load(Path & SubDirStructureWeapons, DataStructureWeapons)
        If Not Result.Success Then
            Data_Load.Problem = Result.Problem
            Exit Function
        End If

        'validate field amounts and name uniqueness

        Dim Entry_Num_List As sFileData_Entry_Num_List = New sFileData_Entry_Num_List
        'check there are the correct number of fields in names data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataNames, 2, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            Data_Load.Problem = "Invalid entries in names.txt."
            Exit Function
        End If
        'check there are no two names for the same thing
        If Not FileData_Field_Check_Unique(DataNames, 0) Then
            Data_Load.Problem = "There are two entries for the same code in names.txt."
            Exit Function
        End If
        'check there are the correct number of fields in structure data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataStructures, 25, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            Data_Load.Problem = "Entries in structures.txt with wrong number of fields."
            Exit Function
        End If
        'check there are no two names for the same thing
        If Not FileData_Field_Check_Unique(DataStructures, 0) Then
            Data_Load.Problem = "There are two entries for the same code in structures.txt."
            Exit Function
        End If
        'check there are the correct number of fields in brain data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataBrain, 9, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            Data_Load.Problem = "Entries in brain.txt with wrong number of fields."
            Exit Function
        End If
        'check there are no two names for the same thing
        If Not FileData_Field_Check_Unique(DataBrain, 0) Then
            Data_Load.Problem = "There are two entries for the same code in brain.txt."
            Exit Function
        End If
        'check there are the correct number of fields in body data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataBody, 25, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            Data_Load.Problem = "Entries in body.txt with wrong number of fields."
            Exit Function
        End If
        'check there are no two names for the same thing
        If Not FileData_Field_Check_Unique(DataBody, 0) Then
            Data_Load.Problem = "There are two entries for the same code in body.txt."
            Exit Function
        End If
        'check there are the correct number of fields in propulsion data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataPropulsion, 12, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            Data_Load.Problem = "Entries in propulsion.txt with wrong number of fields."
            Exit Function
        End If
        'check there are no two names for the same thing
        If Not FileData_Field_Check_Unique(DataPropulsion, 0) Then
            Data_Load.Problem = "There are two entries for the same code in propulsion.txt."
            Exit Function
        End If
        'check there are the correct number of fields in construction data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataConstruction, 12, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            Data_Load.Problem = "Entries in construction.txt with wrong number of fields."
            Exit Function
        End If
        'check there are no two names for the same thing
        If Not FileData_Field_Check_Unique(DataConstruction, 0) Then
            Data_Load.Problem = "There are two entries for the same code in construction.txt."
            Exit Function
        End If
        'check there are the correct number of fields in sensor data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataSensor, 16, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            Data_Load.Problem = "Entries in sensor.txt with wrong number of fields."
            Exit Function
        End If
        'check there are no two names for the same thing
        If Not FileData_Field_Check_Unique(DataSensor, 0) Then
            Data_Load.Problem = "There are two entries for the same code in sensor.txt."
            Exit Function
        End If
        'check there are the correct number of fields in repair data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataRepair, 14, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            Data_Load.Problem = "Entries in repair.txt with wrong number of fields."
            Exit Function
        End If
        'check there are no two names for the same thing
        If Not FileData_Field_Check_Unique(DataRepair, 0) Then
            Data_Load.Problem = "There are two entries for the same code in repair.txt."
            Exit Function
        End If
        'check there are the correct number of fields in templates data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataTemplates, 12, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            Data_Load.Problem = "Entries in templates.txt with wrong number of fields."
            Exit Function
        End If
        'check there are no two names for the same thing
        If Not FileData_Field_Check_Unique(DataTemplates, 0) Then
            Data_Load.Problem = "There are two entries for the same code in templates.txt."
            Exit Function
        End If
        'check there are the correct number of fields in ecm data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataECM, 14, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            Data_Load.Problem = "Entries in ecm.txt with wrong number of fields."
            Exit Function
        End If
        'check there are no two names for the same thing
        If Not FileData_Field_Check_Unique(DataECM, 0) Then
            Data_Load.Problem = "There are two entries for the same code in ecm.txt."
            Exit Function
        End If
        'check there are the correct number of fields in feature data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataFeatures, 11, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            Data_Load.Problem = "Entries in features.txt with wrong number of fields."
            Exit Function
        End If
        'check there are no two names for the same thing
        If Not FileData_Field_Check_Unique(DataFeatures, 0) Then
            Data_Load.Problem = "There are two entries for the same code in features.txt."
            Exit Function
        End If
        'check there are the correct number of fields in assignweapons data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataAssignWeapons, 5, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            Data_Load.Problem = "Entries in assignweapons.txt with wrong number of fields."
            Exit Function
        End If
        'check there are the correct number of fields in weapon data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataWeapons, 53, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            Data_Load.Problem = "Entries in weapons.txt with wrong number of fields."
            Exit Function
        End If
        'check there are no two names for the same thing
        If Not FileData_Field_Check_Unique(DataWeapons, 0) Then
            Data_Load.Problem = "There are two entries for the same code in features.txt."
            Exit Function
        End If
        'check there are the correct number of fields in bodypropulsion data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataBodyPropulsion, 5, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            Data_Load.Problem = "Entries in bodypropulsionimd.txt with wrong number of fields."
            Exit Function
        End If
        'check there are the correct number of fields in structureweapons data
        FileData_Entries_Get_From_FieldCountNotEqualTo(DataStructureWeapons, 6, Entry_Num_List)
        If Entry_Num_List.ResultCount > 0 Then
            Data_Load.Problem = "Entries in structureweapons.txt with wrong number of fields."
            Exit Function
        End If

        'interpret data

        Dim Mod_Data_New As New sMod_Data

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

        With Mod_Data_New

            'interpret body
            With .Body_List
                .BodyCount = DataBody.EntryCount
                ReDim .Bodies(.BodyCount - 1)
                For Body_Num = 0 To .BodyCount - 1
                    .Bodies(Body_Num).Code = DataBody.Entry(Body_Num).FieldValue(0)
                    FileData_Entries_Get_From_Field_Value(DataNames, 0, .Bodies(Body_Num).Code, Entry_Num_List)
                    If Entry_Num_List.ResultCount > 0 Then
                        .Bodies(Body_Num).Name = DataNames.Entry(Entry_Num_List.Result_Entry_Num(0)).FieldValue(1)
                    Else
                        Data_Load.Problem = "No name in names.txt for body component " & .Bodies(Body_Num).Code & "."
                        Exit Function
                    End If
                    .Bodies(Body_Num).PIE = LCase(DataBody.Entry(Body_Num).FieldValue(7))
                Next Body_Num
            End With
            DataBody.EntryCount = 0
            Erase DataBody.Entry

            'interpret propulsion
            With .Propulsion_List
                .PropulsionCount = DataPropulsion.EntryCount
                ReDim .Propulsions(.PropulsionCount - 1)
                For Propulsion_Num = 0 To .PropulsionCount - 1
                    .Propulsions(Propulsion_Num).Code = DataPropulsion.Entry(Propulsion_Num).FieldValue(0)
                    FileData_Entries_Get_From_Field_Value(DataNames, 0, .Propulsions(Propulsion_Num).Code, Entry_Num_List)
                    If Entry_Num_List.ResultCount > 0 Then
                        .Propulsions(Propulsion_Num).Name = DataNames.Entry(Entry_Num_List.Result_Entry_Num(0)).FieldValue(1)
                    Else
                        Data_Load.Problem = "No name in names.txt for propulsion component " & .Propulsions(Propulsion_Num).Code & "."
                        Exit Function
                    End If
                    .Propulsions(Propulsion_Num).PIE = LCase(DataPropulsion.Entry(Propulsion_Num).FieldValue(8))
                Next Propulsion_Num
            End With
            DataPropulsion.EntryCount = 0
            Erase DataPropulsion.Entry

            'interpret construction
            With .Construction_List
                .ConstructionCount = DataConstruction.EntryCount
                ReDim .Constructions(.ConstructionCount - 1)
                For Construction_Num = 0 To .ConstructionCount - 1
                    .Constructions(Construction_Num).Code = DataConstruction.Entry(Construction_Num).FieldValue(0)
                    FileData_Entries_Get_From_Field_Value(DataNames, 0, .Constructions(Construction_Num).Code, Entry_Num_List)
                    If Entry_Num_List.ResultCount > 0 Then
                        .Constructions(Construction_Num).Name = DataNames.Entry(Entry_Num_List.Result_Entry_Num(0)).FieldValue(1)
                    Else
                        Data_Load.Problem = "No name in names.txt for construction component " & .Constructions(Construction_Num).Code & "."
                        Exit Function
                    End If
                    .Constructions(Construction_Num).PIE = LCase(DataConstruction.Entry(Construction_Num).FieldValue(8))
                Next Construction_Num
            End With
            DataConstruction.EntryCount = 0
            Erase DataConstruction.Entry

            'interpret weapons
            Dim WeaponNum As Integer
            With .Weapon_List
                .WeaponCount = DataWeapons.EntryCount
                ReDim .Weapons(.WeaponCount - 1)
                For WeaponNum = 0 To .WeaponCount - 1
                    .Weapons(WeaponNum).Code = DataWeapons.Entry(WeaponNum).FieldValue(0)
                    .Weapons(WeaponNum).PIE = LCase(DataWeapons.Entry(WeaponNum).FieldValue(8))
                    .Weapons(WeaponNum).PIE2 = LCase(DataWeapons.Entry(WeaponNum).FieldValue(9))
                Next WeaponNum
            End With

            'interpret sensor
            With .Sensor_List
                .SensorCount = DataSensor.EntryCount
                ReDim .Sensors(.SensorCount - 1)
                For Sensor_Num = 0 To .SensorCount - 1
                    .Sensors(Sensor_Num).Code = DataSensor.Entry(Sensor_Num).FieldValue(0)
                    FileData_Entries_Get_From_Field_Value(DataNames, 0, .Sensors(Sensor_Num).Code, Entry_Num_List)
                    If Entry_Num_List.ResultCount > 0 Then
                        .Sensors(Sensor_Num).Name = DataNames.Entry(Entry_Num_List.Result_Entry_Num(0)).FieldValue(1)
                    Else
                        Data_Load.Problem = "No name in names.txt for sensor component " & .Sensors(Sensor_Num).Code & "."
                        Exit Function
                    End If
                    .Sensors(Sensor_Num).PIE = LCase(DataSensor.Entry(Sensor_Num).FieldValue(8))
                    .Sensors(Sensor_Num).PIE2 = LCase(DataSensor.Entry(Sensor_Num).FieldValue(9))
                Next Sensor_Num
            End With
            DataSensor.EntryCount = 0
            Erase DataSensor.Entry

            'interpret repair
            With .Repair_List
                .RepairCount = DataRepair.EntryCount
                ReDim .Repairs(.RepairCount - 1)
                For Repair_Num = 0 To .RepairCount - 1
                    .Repairs(Repair_Num).Code = DataRepair.Entry(Repair_Num).FieldValue(0)
                    FileData_Entries_Get_From_Field_Value(DataNames, 0, .Repairs(Repair_Num).Code, Entry_Num_List)
                    If Entry_Num_List.ResultCount > 0 Then
                        .Repairs(Repair_Num).Name = DataNames.Entry(Entry_Num_List.Result_Entry_Num(0)).FieldValue(1)
                    Else
                        Data_Load.Problem = "No name in names.txt for repair component " & .Repairs(Repair_Num).Code & "."
                        Exit Function
                    End If
                    .Repairs(Repair_Num).PIE = LCase(DataRepair.Entry(Repair_Num).FieldValue(9))
                Next Repair_Num
            End With
            DataRepair.EntryCount = 0
            Erase DataRepair.Entry

            'interpret brain
            With .Brain_List
                .BrainCount = DataBrain.EntryCount
                ReDim .Brains(.BrainCount - 1)
                For Brain_Num = 0 To .BrainCount - 1
                    .Brains(Brain_Num).Code = DataBrain.Entry(Brain_Num).FieldValue(0)
                    FileData_Entries_Get_From_Field_Value(DataNames, 0, .Brains(Brain_Num).Code, Entry_Num_List)
                    If Entry_Num_List.ResultCount > 0 Then
                        .Brains(Brain_Num).Name = DataNames.Entry(Entry_Num_List.Result_Entry_Num(0)).FieldValue(1)
                    Else
                        Data_Load.Problem = "No name in names.txt for brain component " & .Brains(Brain_Num).Code & "."
                        Exit Function
                    End If
                Next Brain_Num
            End With
            'we still need brain data

            'interpret ecm
            With .ECM_List
                .ECMCount = DataECM.EntryCount
                ReDim .ECMs(.ECMCount - 1)
                For ECM_Num = 0 To .ECMCount - 1
                    .ECMs(ECM_Num).Code = DataECM.Entry(ECM_Num).FieldValue(0)
                    FileData_Entries_Get_From_Field_Value(DataNames, 0, .ECMs(ECM_Num).Code, Entry_Num_List)
                    If Entry_Num_List.ResultCount > 0 Then
                        .ECMs(ECM_Num).Name = DataNames.Entry(Entry_Num_List.Result_Entry_Num(0)).FieldValue(1)
                    Else
                        Data_Load.Problem = "No name in names.txt for ecm component " & .ECMs(ECM_Num).Code & "."
                        Exit Function
                    End If
                    .ECMs(ECM_Num).PIE = LCase(DataECM.Entry(ECM_Num).FieldValue(8))
                Next ECM_Num
            End With

            'interpret feature
            With .Feature_List
                .FeatureCount = DataFeatures.EntryCount
                ReDim .Features(.FeatureCount - 1)
                For Feature_Num = 0 To .FeatureCount - 1
                    .Features(Feature_Num).Code = DataFeatures.Entry(Feature_Num).FieldValue(0)
                    FileData_Entries_Get_From_Field_Value(DataNames, 0, .Features(Feature_Num).Code, Entry_Num_List)
                    If Entry_Num_List.ResultCount > 0 Then
                        .Features(Feature_Num).Name = DataNames.Entry(Entry_Num_List.Result_Entry_Num(0)).FieldValue(1)
                    Else
                        Data_Load.Problem = "No name in names.txt for feature " & .Features(Feature_Num).Code & "."
                        Exit Function
                    End If
                    .Features(Feature_Num).PIE = LCase(DataFeatures.Entry(Feature_Num).FieldValue(6))
                    .Features(Feature_Num).Footprint.X = DataFeatures.Entry(Feature_Num).FieldValue(1)
                    .Features(Feature_Num).Footprint.Y = DataFeatures.Entry(Feature_Num).FieldValue(2)
                    .Features(Feature_Num).Type = DataFeatures.Entry(Feature_Num).FieldValue(7)
                Next Feature_Num
            End With

            'interpret body-propulsions
            ReDim BodyPropulsions(.Body_List.BodyCount - 1, .Propulsion_List.PropulsionCount - 1)
            For Body_Num = 0 To .Body_List.BodyCount - 1
                For Propulsion_Num = 0 To .Propulsion_List.PropulsionCount - 1
                    BodyPropulsions(Body_Num, Propulsion_Num).LeftPIE = "0"
                    BodyPropulsions(Body_Num, Propulsion_Num).RightPIE = "0"
                Next
            Next
            Dim BodyPropNum As Integer
            For BodyPropNum = 0 To DataBodyPropulsion.EntryCount - 1
                With DataBodyPropulsion.Entry(BodyPropNum)
                    Body_Num = Body_Num_Get_From_Code(Mod_Data_New, DataBodyPropulsion.Entry(BodyPropNum).FieldValue(0))
                    Propulsion_Num = Propulsion_Num_Get_From_Code(Mod_Data_New, DataBodyPropulsion.Entry(BodyPropNum).FieldValue(1))
                    If Body_Num >= 0 And Propulsion_Num >= 0 Then
                        If DataAssignWeapons.Entry(BodyPropNum).FieldValue(2) <> "0" Then
                            BodyPropulsions(Body_Num, Propulsion_Num).LeftPIE = LCase(DataBodyPropulsion.Entry(BodyPropNum).FieldValue(2))
                        End If
                        If DataAssignWeapons.Entry(BodyPropNum).FieldValue(3) <> "0" Then
                            BodyPropulsions(Body_Num, Propulsion_Num).RightPIE = LCase(DataBodyPropulsion.Entry(BodyPropNum).FieldValue(3))
                        End If
                    End If
                End With
            Next

            'interpret templates
            With .Template_List
                .TemplateCount = DataTemplates.EntryCount
                ReDim .Templates(.TemplateCount - 1)
                For Template_Num = 0 To .TemplateCount - 1
                    .Templates(Template_Num).Code = DataTemplates.Entry(Template_Num).FieldValue(0)
                    FileData_Entries_Get_From_Field_Value(DataNames, 0, .Templates(Template_Num).Code, Entry_Num_List)
                    If Entry_Num_List.ResultCount > 0 Then
                        .Templates(Template_Num).Name = DataNames.Entry(Entry_Num_List.Result_Entry_Num(0)).FieldValue(1)
                    Else
                        Data_Load.Problem = "No name in names.txt for template component " & .Templates(Template_Num).Code & "."
                        Exit Function
                    End If
                    .Templates(Template_Num).Body = Body_Num_Get_From_Code(Mod_Data_New, DataTemplates.Entry(Template_Num).FieldValue(2))
                    .Templates(Template_Num).Propulsion = Propulsion_Num_Get_From_Code(Mod_Data_New, DataTemplates.Entry(Template_Num).FieldValue(7))
                    .Templates(Template_Num).Brain = Brain_Num_Get_From_Code(Mod_Data_New, DataTemplates.Entry(Template_Num).FieldValue(3))
                    .Templates(Template_Num).Construction = Construction_Num_Get_From_Code(Mod_Data_New, DataTemplates.Entry(Template_Num).FieldValue(4))
                    .Templates(Template_Num).Repair = Repair_Num_Get_From_Code(Mod_Data_New, DataTemplates.Entry(Template_Num).FieldValue(8))
                    .Templates(Template_Num).ECM = ECM_Num_Get_From_Code(Mod_Data_New, DataTemplates.Entry(Template_Num).FieldValue(5))
                    .Templates(Template_Num).Sensor = Sensor_Num_Get_From_Code(Mod_Data_New, DataTemplates.Entry(Template_Num).FieldValue(10))
                    .Templates(Template_Num).Weapon1 = -1
                    .Templates(Template_Num).Weapon2 = -1
                    .Templates(Template_Num).Weapon3 = -1
                Next (Template_Num)
            End With

            Dim AssignedWeapon_Num As Integer
            For AssignedWeapon_Num = 0 To DataAssignWeapons.EntryCount - 1
                With DataAssignWeapons.Entry(AssignedWeapon_Num)
                    Template_Num = Template_Num_Get_From_Code(Mod_Data_New, DataAssignWeapons.Entry(AssignedWeapon_Num).FieldValue(0))
                    If Template_Num >= 0 Then
                        If DataAssignWeapons.Entry(AssignedWeapon_Num).FieldValue(1) <> "NULL" Then
                            Mod_Data_New.Template_List.Templates(Template_Num).Weapon1 = Weapon_Num_Get_From_Code(Mod_Data_New, DataAssignWeapons.Entry(AssignedWeapon_Num).FieldValue(1))
                        End If
                        If DataAssignWeapons.Entry(AssignedWeapon_Num).FieldValue(2) <> "NULL" Then
                            Mod_Data_New.Template_List.Templates(Template_Num).Weapon2 = Weapon_Num_Get_From_Code(Mod_Data_New, DataAssignWeapons.Entry(AssignedWeapon_Num).FieldValue(2))
                        End If
                        If DataAssignWeapons.Entry(AssignedWeapon_Num).FieldValue(3) <> "NULL" Then
                            Mod_Data_New.Template_List.Templates(Template_Num).Weapon3 = Weapon_Num_Get_From_Code(Mod_Data_New, DataAssignWeapons.Entry(AssignedWeapon_Num).FieldValue(3))
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
                    .Structures(Structure_Num).Code = DataStructures.Entry(Structure_Num).FieldValue(0)
                    FileData_Entries_Get_From_Field_Value(DataNames, 0, .Structures(Structure_Num).Code, Entry_Num_List)
                    If Entry_Num_List.ResultCount > 0 Then
                        .Structures(Structure_Num).Name = DataNames.Entry(Entry_Num_List.Result_Entry_Num(0)).FieldValue(1)
                    Else
                        Data_Load.Problem = "No name in names.txt for structure " & .Structures(Structure_Num).Code & "."
                        Exit Function
                    End If
                    .Structures(Structure_Num).Type = DataStructures.Entry(Structure_Num).FieldValue(1)
                    .Structures(Structure_Num).Footprint.X = DataStructures.Entry(Structure_Num).FieldValue(5)
                    .Structures(Structure_Num).Footprint.Y = DataStructures.Entry(Structure_Num).FieldValue(6)
                    .Structures(Structure_Num).PIE = LCase(DataStructures.Entry(Structure_Num).FieldValue(21))
                    .Structures(Structure_Num).BasePIE = LCase(DataStructures.Entry(Structure_Num).FieldValue(22))
                    .Structures(Structure_Num).Weapon1 = -1
                    .Structures(Structure_Num).Weapon2 = -1
                    .Structures(Structure_Num).Weapon3 = -1
                    .Structures(Structure_Num).Weapon4 = -1
                    .Structures(Structure_Num).ECM = ECM_Num_Get_From_Code(Mod_Data_New, DataStructures.Entry(Structure_Num).FieldValue(18))
                    .Structures(Structure_Num).Sensor = Sensor_Num_Get_From_Code(Mod_Data_New, DataStructures.Entry(Structure_Num).FieldValue(19))
                Next Structure_Num
            End With
            DataStructures.EntryCount = 0
            Erase DataStructures.Entry

            Dim StructureWeaponNum As Integer
            For StructureWeaponNum = 0 To DataStructureWeapons.EntryCount - 1
                With DataStructureWeapons.Entry(StructureWeaponNum)
                    Structure_Num = Structure_Num_Get_From_Code(Mod_Data_New, DataStructureWeapons.Entry(StructureWeaponNum).FieldValue(0))
                    If Structure_Num >= 0 Then
                        If DataStructureWeapons.Entry(StructureWeaponNum).FieldValue(1) <> "NULL" Then
                            Mod_Data_New.Structure_List.Structures(Structure_Num).Weapon1 = Weapon_Num_Get_From_Code(Mod_Data_New, DataStructureWeapons.Entry(StructureWeaponNum).FieldValue(1))
                        End If
                        If DataStructureWeapons.Entry(StructureWeaponNum).FieldValue(2) <> "NULL" Then
                            Mod_Data_New.Structure_List.Structures(Structure_Num).Weapon2 = Weapon_Num_Get_From_Code(Mod_Data_New, DataStructureWeapons.Entry(StructureWeaponNum).FieldValue(2))
                        End If
                        If DataStructureWeapons.Entry(StructureWeaponNum).FieldValue(3) <> "NULL" Then
                            Mod_Data_New.Structure_List.Structures(Structure_Num).Weapon3 = Weapon_Num_Get_From_Code(Mod_Data_New, DataStructureWeapons.Entry(StructureWeaponNum).FieldValue(3))
                        End If
                        If DataStructureWeapons.Entry(StructureWeaponNum).FieldValue(4) <> "NULL" Then
                            Mod_Data_New.Structure_List.Structures(Structure_Num).Weapon4 = Weapon_Num_Get_From_Code(Mod_Data_New, DataStructureWeapons.Entry(StructureWeaponNum).FieldValue(4))
                        End If
                    Else
                        'Data_Load.Problem = "Weapons assigned to missing template " & DataAssignWeapons.Entry(AssignedWeapon_Num).FieldValue(0) & "."
                        'Exit Function
                    End If
                End With
            Next
        End With

        'set the main variables to the new variables

        Mod_Data_Main = Mod_Data_New

        'load texpages

        Dim TexFiles() As String

        Try
            TexFiles = IO.Directory.GetFiles(Path & SubDirTexpages)
        Catch ex As Exception
            Data_Load.Problem = ex.Message
            Exit Function
        End Try

        Dim TexFile_Num As Integer
        Dim tmpString As String
        Dim tmpBitmap As clsFileBitmap
        Dim InstrPos2 As Integer

        TexturePageCount = 0
        For TexFile_Num = 0 To TexFiles.GetUpperBound(0)
            tmpString = TexFiles(TexFile_Num)
            If LCase(Right(tmpString, 4)) = ".png" Then
                If IO.File.Exists(tmpString) Then
                    ReDim Preserve TexturePages(TexturePageCount)
                    tmpBitmap = New clsFileBitmap()
                    Result = tmpBitmap.Load(tmpString)
                    If Not Result.Success Then
                        Data_Load.Problem = "Failed loading " & tmpString & "; " & Result.Problem
                        Exit Function
                    End If
                    TexturePages(TexturePageCount).GLTexture_Num = tmpBitmap.GL_Texture_Create()
                    InstrPos2 = InStrRev(tmpString, OSPathSeperator)
                    TexturePages(TexturePageCount).FileTitle = Mid(tmpString, InstrPos2 + 1, tmpString.Length - 4 - InstrPos2)
                    TexturePageCount += 1
                Else
                    Data_Load.Problem = "Texture page missing (" & tmpString & ")."
                    Exit Function
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
            Data_Load.Problem = "Unable to access PIE files."
            Exit Function
        End Try

        ReDim PIE_List.PIEs(PIE_Files.GetUpperBound(0))

        Dim SplitPath As sSplitPath

        For A = 0 To PIE_Files.GetUpperBound(0)
            SplitPath = New sSplitPath(PIE_Files(A))
            If LCase(SplitPath.FileExtension) = "pie" Then
                With PIE_List.PIEs(PIE_List.PIECount)
                    .Path = PIE_Files(A)
                    .LCaseFileTitle = LCase(SplitPath.FileTitle)
                End With
                PIE_List.PIECount += 1
            End If
        Next
        ReDim Preserve PIE_List.PIEs(PIE_List.PIECount - 1)

        Dim tmpAttachment As clsUnitType.clsLoadedInfo.clsAttachment
        Dim tmpBaseAttachment As clsUnitType.clsLoadedInfo.clsAttachment
        Dim tmpConnector As sXYZ_sng

        With Mod_Data_Main
            UnitTypeCount = .Structure_List.StructureCount + .Feature_List.FeatureCount + .Template_List.TemplateCount
            ReDim UnitTypes(UnitTypeCount - 1)
            For A = 0 To .Structure_List.StructureCount - 1
                UnitTypes(C) = New clsUnitType(C)
                UnitTypes(C).Type = clsUnitType.enumType.PlayerStructure
                UnitTypes(C).Code = .Structure_List.Structures(A).Code
                UnitTypes(C).LoadedInfo = New clsUnitType.clsLoadedInfo
                UnitTypes(C).LoadedInfo.Num = C
                UnitTypes(C).LoadedInfo.Name = .Structure_List.Structures(A).Name
                UnitTypes(C).LoadedInfo.Footprint = .Structure_List.Structures(A).Footprint
                If .Structure_List.Structures(A).Type = "DEMOLISH" Then
                    UnitTypes(C).LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.Demolish
                ElseIf .Structure_List.Structures(A).Type = "WALL" Then
                    UnitTypes(C).LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.Wall
                ElseIf .Structure_List.Structures(A).Type = "CORNER WALL" Then
                    UnitTypes(C).LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.CornerWall
                ElseIf .Structure_List.Structures(A).Type = "FACTORY" Then
                    UnitTypes(C).LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.Factory
                ElseIf .Structure_List.Structures(A).Type = "CYBORG FACTORY" Then
                    UnitTypes(C).LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.CyborgFactory
                ElseIf .Structure_List.Structures(A).Type = "VTOL FACTORY" Then
                    UnitTypes(C).LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.VTOLFactory
                ElseIf .Structure_List.Structures(A).Type = "COMMAND" Then
                    UnitTypes(C).LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.Command
                ElseIf .Structure_List.Structures(A).Type = "HQ" Then
                    UnitTypes(C).LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.HQ
                ElseIf .Structure_List.Structures(A).Type = "DEFENSE" Then
                    UnitTypes(C).LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.Defense
                ElseIf .Structure_List.Structures(A).Type = "POWER GENERATOR" Then
                    UnitTypes(C).LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.PowerGenerator
                ElseIf .Structure_List.Structures(A).Type = "POWER MODULE" Then
                    UnitTypes(C).LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.PowerModule
                ElseIf .Structure_List.Structures(A).Type = "RESEARCH" Then
                    UnitTypes(C).LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.Research
                ElseIf .Structure_List.Structures(A).Type = "RESEARCH MODULE" Then
                    UnitTypes(C).LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.ResearchModule
                ElseIf .Structure_List.Structures(A).Type = "FACTORY MODULE" Then
                    UnitTypes(C).LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.FactoryModule
                ElseIf .Structure_List.Structures(A).Type = "DOOR" Then
                    UnitTypes(C).LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.DOOR
                ElseIf .Structure_List.Structures(A).Type = "REPAIR FACILITY" Then
                    UnitTypes(C).LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.RepairFacility
                ElseIf .Structure_List.Structures(A).Type = "SAT UPLINK" Then
                    UnitTypes(C).LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.DOOR
                ElseIf .Structure_List.Structures(A).Type = "REARM PAD" Then
                    UnitTypes(C).LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.RearmPad
                ElseIf .Structure_List.Structures(A).Type = "MISSILE SILO" Then
                    UnitTypes(C).LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.MissileSilo
                ElseIf .Structure_List.Structures(A).Type = "RESOURCE EXTRACTOR" Then
                    UnitTypes(C).LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.ResourceExtractor
                Else
                    UnitTypes(C).LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.Unknown
                End If

                UnitTypes(C).LoadedInfo.BaseAttachment = New clsUnitType.clsLoadedInfo.clsAttachment
                tmpBaseAttachment = UnitTypes(C).LoadedInfo.BaseAttachment
                tmpString = .Structure_List.Structures(A).PIE
                tmpBaseAttachment.AddModel(GetModelForPIE(PIE_List, tmpString, Path & SubDirPIEs & tmpString))
                tmpString = .Structure_List.Structures(A).BasePIE
                UnitTypes(C).LoadedInfo.StructureBasePlate = GetModelForPIE(PIE_List, tmpString, Path & SubDirPIEs & tmpString)
                If tmpBaseAttachment.ModelCount = 1 Then
                    If tmpBaseAttachment.Models(0).ConnectorCount >= 1 Then
                        tmpConnector = tmpBaseAttachment.Models(0).Connectors(0)
                        If .Structure_List.Structures(A).Weapon1 >= 0 Then
                            If .Weapon_List.Weapons(.Structure_List.Structures(A).Weapon1).Code <> "ZNULLWEAPON" Then
                                tmpString = .Weapon_List.Weapons(.Structure_List.Structures(A).Weapon1).PIE
                                tmpAttachment = tmpBaseAttachment.CreateAttachment()
                                tmpAttachment.AddModel(GetModelForPIE(PIE_List, tmpString, Path & SubDirPIEs & tmpString))
                                tmpAttachment.Pos_Offset = tmpConnector

                                tmpString = .Weapon_List.Weapons(.Structure_List.Structures(A).Weapon1).PIE2
                                tmpAttachment = tmpBaseAttachment.CreateAttachment()
                                tmpAttachment.AddModel(GetModelForPIE(PIE_List, tmpString, Path & SubDirPIEs & tmpString))
                                tmpAttachment.Pos_Offset = tmpConnector
                            End If
                        End If
                        If .Structure_List.Structures(A).ECM >= 0 Then
                            If .ECM_List.ECMs(.Structure_List.Structures(A).ECM).Code <> "ZNULLECM" Then
                                tmpString = .ECM_List.ECMs(.Structure_List.Structures(A).ECM).PIE
                                tmpAttachment = tmpBaseAttachment.CreateAttachment()
                                tmpAttachment.AddModel(GetModelForPIE(PIE_List, tmpString, Path & SubDirPIEs & tmpString))
                                tmpAttachment.Pos_Offset = tmpConnector
                            End If
                        End If
                        If .Structure_List.Structures(A).Sensor >= 0 Then
                            If .Sensor_List.Sensors(.Structure_List.Structures(A).Sensor).Code <> "ZNULLSENSOR" Then
                                tmpString = .Sensor_List.Sensors(.Structure_List.Structures(A).Sensor).PIE
                                tmpAttachment = tmpBaseAttachment.CreateAttachment()
                                tmpAttachment.AddModel(GetModelForPIE(PIE_List, tmpString, Path & SubDirPIEs & tmpString))
                                tmpAttachment.Pos_Offset = tmpConnector

                                tmpString = .Sensor_List.Sensors(.Structure_List.Structures(A).Sensor).PIE2
                                tmpAttachment = tmpBaseAttachment.CreateAttachment()
                                tmpAttachment.AddModel(GetModelForPIE(PIE_List, tmpString, Path & SubDirPIEs & tmpString))
                                tmpAttachment.Pos_Offset = tmpConnector
                            End If
                        End If
                    End If
                End If
                C += 1
            Next
            For A = 0 To .Feature_List.FeatureCount - 1
                UnitTypes(C) = New clsUnitType(C)
                UnitTypes(C).Type = clsUnitType.enumType.Feature
                UnitTypes(C).Code = .Feature_List.Features(A).Code
                UnitTypes(C).LoadedInfo = New clsUnitType.clsLoadedInfo
                UnitTypes(C).LoadedInfo.Num = C
                If .Feature_List.Features(A).Type = "OIL RESOURCE" Then
                    UnitTypes(C).LoadedInfo.StructureType = clsUnitType.clsLoadedInfo.enumStructureType.OilResource
                End If
                UnitTypes(C).LoadedInfo.Name = .Feature_List.Features(A).Name
                UnitTypes(C).LoadedInfo.Footprint = .Feature_List.Features(A).Footprint
                UnitTypes(C).LoadedInfo.BaseAttachment = New clsUnitType.clsLoadedInfo.clsAttachment
                tmpBaseAttachment = UnitTypes(C).LoadedInfo.BaseAttachment
                tmpString = .Feature_List.Features(A).PIE
                tmpAttachment = tmpBaseAttachment.CreateAttachment()
                tmpAttachment.AddModel(GetModelForPIE(PIE_List, tmpString, Path & SubDirPIEs & tmpString))
                C += 1
            Next
            For A = 0 To .Template_List.TemplateCount - 1
                UnitTypes(C) = New clsUnitType(C)
                UnitTypes(C).Type = clsUnitType.enumType.PlayerDroidTemplate
                UnitTypes(C).Code = .Template_List.Templates(A).Code
                UnitTypes(C).LoadedInfo = New clsUnitType.clsLoadedInfo
                UnitTypes(C).LoadedInfo.Num = C
                UnitTypes(C).LoadedInfo.Name = .Template_List.Templates(A).Name
                UnitTypes(C).LoadedInfo.Footprint.X = 1
                UnitTypes(C).LoadedInfo.Footprint.Y = 1
                If .Template_List.Templates(A).Body >= 0 Then
                    UnitTypes(C).LoadedInfo.BaseAttachment = New clsUnitType.clsLoadedInfo.clsAttachment
                    tmpBaseAttachment = UnitTypes(C).LoadedInfo.BaseAttachment
                    tmpString = .Body_List.Bodies(.Template_List.Templates(A).Body).PIE
                    tmpBaseAttachment.AddModel(GetModelForPIE(PIE_List, tmpString, Path & SubDirPIEs & tmpString))
                    If .Template_List.Templates(A).Propulsion >= 0 Then
                        tmpString = BodyPropulsions(.Template_List.Templates(A).Body, .Template_List.Templates(A).Propulsion).LeftPIE
                        tmpAttachment = tmpBaseAttachment.CreateAttachment()
                        tmpAttachment.AddModel(GetModelForPIE(PIE_List, tmpString, Path & SubDirPIEs & tmpString))

                        tmpString = BodyPropulsions(.Template_List.Templates(A).Body, .Template_List.Templates(A).Propulsion).RightPIE
                        tmpAttachment = tmpBaseAttachment.CreateAttachment()
                        tmpAttachment.AddModel(GetModelForPIE(PIE_List, tmpString, Path & SubDirPIEs & tmpString))
                    End If
                    If tmpBaseAttachment.ModelCount = 1 Then
                        If tmpBaseAttachment.Models(0).ConnectorCount >= 1 Then
                            tmpConnector = tmpBaseAttachment.Models(0).Connectors(0)
                            If .Template_List.Templates(A).Brain >= 0 Then
                                If .Brain_List.Brains(.Template_List.Templates(A).Brain).Code <> "ZNULLBRAIN" Then
                                    tmpString = .Brain_List.Brains(.Template_List.Templates(A).Brain).PIE
                                    tmpAttachment = tmpBaseAttachment.CreateAttachment()
                                    tmpAttachment.AddModel(GetModelForPIE(PIE_List, tmpString, Path & SubDirPIEs & tmpString))
                                    tmpAttachment.Pos_Offset = tmpConnector
                                End If
                            End If
                            If .Template_List.Templates(A).Construction >= 0 Then
                                If .Construction_List.Constructions(.Template_List.Templates(A).Construction).Code <> "ZNULLCONSTRUCT" Then
                                    tmpString = .Construction_List.Constructions(.Template_List.Templates(A).Construction).PIE
                                    tmpAttachment = tmpBaseAttachment.CreateAttachment()
                                    tmpAttachment.AddModel(GetModelForPIE(PIE_List, tmpString, Path & SubDirPIEs & tmpString))
                                    tmpAttachment.Pos_Offset = tmpConnector
                                End If
                            End If
                            If .Template_List.Templates(A).ECM >= 0 Then
                                If .ECM_List.ECMs(.Template_List.Templates(A).ECM).Code <> "ZNULLECM" Then
                                    tmpString = .ECM_List.ECMs(.Template_List.Templates(A).ECM).PIE
                                    tmpAttachment = tmpBaseAttachment.CreateAttachment()
                                    tmpAttachment.AddModel(GetModelForPIE(PIE_List, tmpString, Path & SubDirPIEs & tmpString))
                                    tmpAttachment.Pos_Offset = tmpConnector
                                End If
                            End If
                            If .Template_List.Templates(A).Repair >= 0 Then
                                If .Repair_List.Repairs(.Template_List.Templates(A).Repair).Code <> "ZNULLREPAIR" Then
                                    tmpString = .Repair_List.Repairs(.Template_List.Templates(A).Repair).PIE
                                    tmpAttachment = tmpBaseAttachment.CreateAttachment()
                                    tmpAttachment.AddModel(GetModelForPIE(PIE_List, tmpString, Path & SubDirPIEs & tmpString))
                                    tmpAttachment.Pos_Offset = tmpConnector
                                End If
                            End If
                            If .Template_List.Templates(A).Sensor >= 0 Then
                                If .Sensor_List.Sensors(.Template_List.Templates(A).Sensor).Code <> "ZNULLSENSOR" Then
                                    tmpString = .Sensor_List.Sensors(.Template_List.Templates(A).Sensor).PIE
                                    tmpAttachment = tmpBaseAttachment.CreateAttachment()
                                    tmpAttachment.AddModel(GetModelForPIE(PIE_List, tmpString, Path & SubDirPIEs & tmpString))
                                    tmpAttachment.Pos_Offset = tmpConnector

                                    tmpString = .Sensor_List.Sensors(.Template_List.Templates(A).Sensor).PIE2
                                    tmpAttachment = tmpBaseAttachment.CreateAttachment()
                                    tmpAttachment.AddModel(GetModelForPIE(PIE_List, tmpString, Path & SubDirPIEs & tmpString))
                                    tmpAttachment.Pos_Offset = tmpConnector
                                End If
                            End If
                            If .Template_List.Templates(A).Weapon1 >= 0 Then
                                If .Weapon_List.Weapons(.Template_List.Templates(A).Weapon1).Code <> "ZNULLWEAPON" Then
                                    tmpString = .Weapon_List.Weapons(.Template_List.Templates(A).Weapon1).PIE
                                    tmpAttachment = tmpBaseAttachment.CreateAttachment()
                                    tmpAttachment.AddModel(GetModelForPIE(PIE_List, tmpString, Path & SubDirPIEs & tmpString))
                                    tmpAttachment.Pos_Offset = tmpConnector

                                    tmpString = .Weapon_List.Weapons(.Template_List.Templates(A).Weapon1).PIE2
                                    tmpAttachment = tmpBaseAttachment.CreateAttachment()
                                    tmpAttachment.AddModel(GetModelForPIE(PIE_List, tmpString, Path & SubDirPIEs & tmpString))
                                    tmpAttachment.Pos_Offset = tmpConnector
                                End If
                            End If
                        End If
                    End If
                End If
                C += 1
            Next
        End With

        Data_Load.Success = True
    End Function

    Function FileData_Field_Check_Unique(ByRef FileData As sFileData, ByVal Field_Num As Integer) As Boolean
        FileData_Field_Check_Unique = False

        Dim Entry_Num As Integer
        Dim Entry_Num_Other As Integer

        With FileData
            For Entry_Num = 0 To .EntryCount - 1
                For Entry_Num_Other = Entry_Num + 1 To .EntryCount - 1
                    If .Entry(Entry_Num).FieldValue(Field_Num) = .Entry(Entry_Num_Other).FieldValue(Field_Num) Then
                        Return False
                    End If
                Next Entry_Num_Other
            Next Entry_Num
        End With
        Return True
    End Function

    Sub FileData_Entries_Get_From_FieldCountNotEqualTo(ByRef FileData As sFileData, ByVal FieldCount As Integer, ByRef Output_Entry_Num_List As sFileData_Entry_Num_List)
        Dim Entry_Num As Integer

        Output_Entry_Num_List.ResultCount = 0
        ReDim Output_Entry_Num_List.Result_Entry_Num(FileData.EntryCount - 1)
        With FileData
            For Entry_Num = 0 To .EntryCount - 1
                With .Entry(Entry_Num)
                    If Not .FieldCount = FieldCount Then
                        ReDim Preserve Output_Entry_Num_List.Result_Entry_Num(Output_Entry_Num_List.ResultCount)
                        Output_Entry_Num_List.Result_Entry_Num(Output_Entry_Num_List.ResultCount) = Entry_Num
                        Output_Entry_Num_List.ResultCount += 1
                    End If
                End With
            Next Entry_Num
        End With
        ReDim Preserve Output_Entry_Num_List.Result_Entry_Num(Output_Entry_Num_List.ResultCount - 1)
    End Sub

    Sub FileData_Entries_Get_From_Not_FieldCount_InRange(ByRef FileData As sFileData, ByVal FieldCountMin As Integer, ByVal FieldCountMax As Integer, ByRef Output_Entry_Num_List As sFileData_Entry_Num_List)
        Dim Entry_Num As Integer

        Output_Entry_Num_List.ResultCount = 0
        ReDim Output_Entry_Num_List.Result_Entry_Num(FileData.EntryCount - 1)
        With FileData
            For Entry_Num = 0 To .EntryCount - 1
                With .Entry(Entry_Num)
                    If (.FieldCount < FieldCountMin Or .FieldCount > FieldCountMax) Then
                        ReDim Preserve Output_Entry_Num_List.Result_Entry_Num(Output_Entry_Num_List.ResultCount)
                        Output_Entry_Num_List.Result_Entry_Num(Output_Entry_Num_List.ResultCount) = Entry_Num
                        Output_Entry_Num_List.ResultCount += 1
                    End If
                End With
            Next Entry_Num
        End With
        ReDim Preserve Output_Entry_Num_List.Result_Entry_Num(Output_Entry_Num_List.ResultCount - 1)
    End Sub

    Sub FileData_Entries_Get_From_Field_Value(ByRef FileData As sFileData, ByVal Search_Field_Num As Integer, ByVal Search_String As String, ByRef Output_Entry_Num_List As sFileData_Entry_Num_List)
        Dim Entry_Num As Integer

        Output_Entry_Num_List.ResultCount = 0
        ReDim Output_Entry_Num_List.Result_Entry_Num(FileData.EntryCount - 1)
        With FileData
            For Entry_Num = 0 To .EntryCount - 1
                With .Entry(Entry_Num)
                    If .FieldValue(Search_Field_Num) = Search_String Then
                        Output_Entry_Num_List.Result_Entry_Num(Output_Entry_Num_List.ResultCount) = Entry_Num
                        Output_Entry_Num_List.ResultCount += 1
                    End If
                End With
            Next Entry_Num
        End With
        ReDim Preserve Output_Entry_Num_List.Result_Entry_Num(Output_Entry_Num_List.ResultCount - 1)
    End Sub

    Sub BytesToLines(ByRef Bytes() As Byte, ByRef OutputLines() As String)
        Dim ByteCount As Integer = Bytes.GetUpperBound(0) + 1
        Dim StartNum As Integer
        Dim Length As Integer
        Dim ByteNum As Integer
        Dim LineCount As Integer = OutputLines.GetUpperBound(0) + 1
        Dim CharByteNum As Integer
        Dim Chars(511) As Char
        Dim Count As Integer

        StartNum = 0
        Do While StartNum < ByteCount
            ByteNum = StartNum
            Do
                If ByteNum >= ByteCount Then
                    Length = ByteNum - StartNum
                    Exit Do
                ElseIf Bytes(ByteNum) = 13 Then
                    Length = ByteNum - StartNum
                    ByteNum += 1
                    If ByteNum >= ByteCount Then

                    ElseIf Bytes(ByteNum) = 10 Then
                        ByteNum += 1
                    End If
                    Exit Do
                ElseIf Bytes(ByteNum) = 10 Then
                    Length = ByteNum - StartNum
                    ByteNum += 1
                    Exit Do
                Else
                    ByteNum += 1
                End If
            Loop

            If OutputLines.GetUpperBound(0) < LineCount Then
                ReDim Preserve OutputLines(LineCount + 127)
            End If
            If Length > 0 Then
                If Chars.GetUpperBound(0) < Length - 1 Then
                    ReDim Chars(Length - 1)
                End If
                Count = 0
                For CharByteNum = StartNum To StartNum + Length - 1
                    Chars(Count) = Chr(Bytes(CharByteNum))
                    Count += 1
                Next
                OutputLines(LineCount) = New String(Chars, 0, Length)
            Else
                OutputLines(LineCount) = ""
            End If
            LineCount += 1

            StartNum = ByteNum
        Loop
        ReDim Preserve OutputLines(LineCount - 1)
    End Sub

    Sub Lines_Remove_Comments(ByRef Lines() As String)
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

    Function Comma_File_Load(ByVal Path As String, ByRef FileData As sFileData) As sResult
        Comma_File_Load.Problem = ""
        Comma_File_Load.Success = False

        Dim Bytes() As Byte
        Dim LineData(-1) As String
        Dim LineCount As Integer
        Dim LineNum As Integer
        Dim strTemp As String
        Dim CommaPos As Short
        Dim Flag As Boolean

        'load all bytes
        Try
            Bytes = IO.File.ReadAllBytes(Path)
        Catch ex As Exception
            Comma_File_Load.Problem = ex.Message
            Exit Function
        End Try

        BytesToLines(Bytes, LineData)

        Lines_Remove_Comments(LineData)
        LineCount = LineData.GetUpperBound(0) + 1

        With FileData
            ReDim .Entry(LineCount - 1)
            For LineNum = 0 To LineCount - 1
                strTemp = LineData(LineNum)
                Flag = False
                With .Entry(.EntryCount)
                    ReDim .FieldValue(63)
                    Do
                        If .FieldValue.GetUpperBound(0) < .FieldCount Then
                            ReDim Preserve .FieldValue(.FieldCount + 16)
                        End If
                        CommaPos = InStr(1, strTemp, ",")
                        If CommaPos = 0 Then
                            .FieldValue(.FieldCount) = strTemp
                            Flag = True
                        Else
                            .FieldValue(.FieldCount) = Left(strTemp, CommaPos - 1)
                            strTemp = Right(strTemp, strTemp.Length - CommaPos)
                        End If
                        .FieldCount += 1
                        If Flag Then
                            ReDim Preserve .FieldValue(.FieldCount - 1)
                            Exit Do
                        End If
                    Loop
                End With
                .EntryCount += 1
            Next LineNum
            ReDim Preserve .Entry(.EntryCount - 1)
        End With

        Comma_File_Load.Success = True
    End Function

    Function Names_File_Load(ByVal Path As String, ByRef FileData As sFileData) As sResult
        Names_File_Load.Problem = ""
        Names_File_Load.Success = False

        Dim Bytes() As Byte
        Dim LineData(-1) As String
        Dim LineCount As Integer
        Dim LineNum As Integer
        Dim strTemp As String
        Dim A As Integer
        Dim B As Integer
        Dim tmpChar As Char

        'load all bytes
        Try
            Bytes = IO.File.ReadAllBytes(Path)
        Catch ex As Exception
            Names_File_Load.Problem = ex.Message
            Exit Function
        End Try

        BytesToLines(Bytes, LineData)

        Lines_Remove_Comments(LineData)
        LineCount = LineData.GetUpperBound(0) + 1

        'output as entries with fields
        Dim Code As String
        With FileData
            ReDim .Entry(LineCount - 1)
            For LineNum = 0 To LineCount - 1
                strTemp = LineData(LineNum)
                'get code until space or tab
                For A = 0 To strTemp.Length - 1
                    tmpChar = strTemp.Chars(A)
                    If tmpChar = " "c Or Asc(tmpChar) = 9 Then
                        Exit For
                    End If
                Next A
                Code = Left(strTemp, A)
                If Code <> "" Then
                    With .Entry(.EntryCount)
                        .FieldCount = 2
                        ReDim Preserve .FieldValue(.FieldCount - 1)
                        .FieldValue(0) = Code
                        'ignore everything until the quotation mark
                        For B = A + 1 To strTemp.Length - 1
                            tmpChar = strTemp.Chars(B)
                            If Asc(tmpChar) = 34 Then
                                Exit For
                            End If
                        Next B
                        'get name until second quotation mark
                        For A = B + 1 To strTemp.Length - 1
                            tmpChar = strTemp.Chars(A)
                            If Asc(tmpChar) = 34 Then
                                Exit For
                            End If
                        Next A
                        If B + 1 < strTemp.Length Then
                            .FieldValue(1) = strTemp.Substring(B + 1, A - (B + 1))
                        Else
                            .FieldValue(1) = ""
                        End If
                    End With
                    .EntryCount += 1
                End If
            Next LineNum
            ReDim Preserve .Entry(.EntryCount - 1)
        End With

        Names_File_Load.Success = True
    End Function

    Function Construction_Num_Get_From_Code(ByRef Mod_Data As sMod_Data, ByVal Code As String) As Integer
        Dim Construction_Num As Integer

        With Mod_Data.Construction_List
            For Construction_Num = 0 To .ConstructionCount - 1
                If .Constructions(Construction_Num).Code = Code Then
                    Construction_Num_Get_From_Code = Construction_Num
                    Exit Function
                End If
            Next
        End With
        Construction_Num_Get_From_Code = -1
    End Function

    Function Body_Num_Get_From_Code(ByRef Mod_Data As sMod_Data, ByVal Code As String) As Integer
        Dim Body_Num As Integer

        With Mod_Data.Body_List
            For Body_Num = 0 To .BodyCount - 1
                If .Bodies(Body_Num).Code = Code Then
                    Body_Num_Get_From_Code = Body_Num
                    Exit Function
                End If
            Next
        End With
        Body_Num_Get_From_Code = -1
    End Function

    Function Brain_Num_Get_From_Code(ByRef Mod_Data As sMod_Data, ByVal Code As String) As Integer
        Dim Brain_Num As Integer

        With Mod_Data.Brain_List
            For Brain_Num = 0 To .BrainCount - 1
                If .Brains(Brain_Num).Code = Code Then
                    Brain_Num_Get_From_Code = Brain_Num
                    Exit Function
                End If
            Next
        End With
        Brain_Num_Get_From_Code = -1
    End Function

    Function Propulsion_Num_Get_From_Code(ByRef Mod_Data As sMod_Data, ByVal Code As String) As Integer
        Dim Propulsion_Num As Integer

        With Mod_Data.Propulsion_List
            For Propulsion_Num = 0 To .PropulsionCount - 1
                If .Propulsions(Propulsion_Num).Code = Code Then
                    Propulsion_Num_Get_From_Code = Propulsion_Num
                    Exit Function
                End If
            Next
        End With
        Propulsion_Num_Get_From_Code = -1
    End Function

    Function Repair_Num_Get_From_Code(ByRef Mod_Data As sMod_Data, ByVal Code As String) As Integer
        Dim Repair_Num As Integer

        With Mod_Data.Repair_List
            For Repair_Num = 0 To .RepairCount - 1
                If .Repairs(Repair_Num).Code = Code Then
                    Repair_Num_Get_From_Code = Repair_Num
                    Exit Function
                End If
            Next
        End With
        Repair_Num_Get_From_Code = -1
    End Function

    Function ECM_Num_Get_From_Code(ByRef Mod_Data As sMod_Data, ByVal Code As String) As Integer
        Dim ECM_Num As Integer

        With Mod_Data.ECM_List
            For ECM_Num = 0 To .ECMCount - 1
                If .ECMs(ECM_Num).Code = Code Then
                    ECM_Num_Get_From_Code = ECM_Num
                    Exit Function
                End If
            Next
        End With
        ECM_Num_Get_From_Code = -1
    End Function

    Function Sensor_Num_Get_From_Code(ByRef Mod_Data As sMod_Data, ByVal Code As String) As Integer
        Dim Sensor_Num As Integer

        With Mod_Data.Sensor_List
            For Sensor_Num = 0 To .SensorCount - 1
                If .Sensors(Sensor_Num).Code = Code Then
                    Sensor_Num_Get_From_Code = Sensor_Num
                    Exit Function
                End If
            Next
        End With
        Sensor_Num_Get_From_Code = -1
    End Function

    Function Weapon_Num_Get_From_Code(ByRef Mod_Data As sMod_Data, ByVal Code As String) As Integer
        Dim Weapon_Num As Integer

        With Mod_Data.Weapon_List
            For Weapon_Num = 0 To .WeaponCount - 1
                If .Weapons(Weapon_Num).Code = Code Then
                    Weapon_Num_Get_From_Code = Weapon_Num
                    Exit Function
                End If
            Next
        End With
        Weapon_Num_Get_From_Code = -1
    End Function

    Function Template_Num_Get_From_Code(ByRef Mod_Data As sMod_Data, ByVal Code As String) As Integer
        Dim Template_Num As Integer

        With Mod_Data.Template_List
            For Template_Num = 0 To .TemplateCount - 1
                If .Templates(Template_Num).Code = Code Then
                    Template_Num_Get_From_Code = Template_Num
                    Exit Function
                End If
            Next
        End With
        Template_Num_Get_From_Code = -1
    End Function

    Function Structure_Num_Get_From_Code(ByRef Mod_Data As sMod_Data, ByVal Code As String) As Integer
        Dim Structure_Num As Integer

        With Mod_Data.Structure_List
            For Structure_Num = 0 To .StructureCount - 1
                If .Structures(Structure_Num).Code = Code Then
                    Structure_Num_Get_From_Code = Structure_Num
                    Exit Function
                End If
            Next
        End With
        Structure_Num_Get_From_Code = -1
    End Function

    Function GetModelForPIE(ByRef PIE_List As sPIE_List, ByVal PIE_LCaseFileTitle As String, ByVal PIE_FullPath As String) As clsModel

        If PIE_LCaseFileTitle = "0" Then
            Return Nothing
        End If

        Dim A As Integer

        For A = 0 To PIE_List.PIECount - 1
            If PIE_List.PIEs(A).LCaseFileTitle = PIE_LCaseFileTitle Then
                If PIE_List.PIEs(A).Model Is Nothing Then
                    PIE_List.PIEs(A).Model = New clsModel
                    PIE_List.PIEs(A).Model.LoadPIE(PIE_FullPath)
                End If
                Return PIE_List.PIEs(A).Model
            End If
        Next
        Return Nothing
    End Function
End Module