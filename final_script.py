import requests
from datetime import datetime

GREEDY_URL = "https://localhost:44345/api/Algorithms/Greedy"
LARGEST_FIRST_URL = "https://localhost:44345/api/Algorithms/LargestFirst"
ABC_URL = "https://localhost:44345/api/Algorithms/ABC"

array = [0,0,0,0,0]

def performGreedyTests(_graphId, _testCount):
    print(f"*************** PERFORM GREEDY ALGORITHM ***************, {_testCount}")
    for i in range(_testCount):
        print(f"{datetime.now().time()}, ##### {i} #####")
        PARAMS_G_LF = {'graphId': _graphId}
        print('##### PARAMS #####')
        print(PARAMS_G_LF)
        print('##################')
        print('##### MAKE REQUEST - GREEDY #####')
        r = requests.get(url = GREEDY_URL, params = PARAMS_G_LF, verify=False)
        print('##### RESPONSE #####')
        j = r.json()
        a_set = set(j['coloredNodes'])        
        print(len(a_set))
    print("***************** END GREEDY ALGORITHM *****************")    

def performLargestFirst(_graphId):
    print("*************** PERFORM LARGEST FIRST ALGORITHM ***************")
    PARAMS_G_LF = {'graphId': _graphId}
    print('##### PARAMS #####')
    print(PARAMS_G_LF)
    print('##################')
    print('##### MAKE REQUEST - LARGEST FIRST #####')
    r = requests.get(url = LARGEST_FIRST_URL, params = PARAMS_G_LF, verify=False)
    print('##### RESPONSE #####')
    j = r.json()
    a_set = set(j['coloredNodes'])   
    print(len(a_set))
    print("***************** END LARGEST FIRST ALGORITHM *****************")    

def performABC(_graphId, _testCount, _emplSize, _emplNeig, _onlookSize, _onLookNeig, _scoutSize, _cicles, _onLookerFavSolutionNmb,idx):
    global array
    print(f"*************** PERFORM ABC ALGORITHM ***************, {_testCount}")
    for i in range(_testCount):
        print(f"{datetime.now().time()}, ##### {i} #####")
        PARAMS_ABC = {'graphId':_graphId, 'employeeBeesSize': _emplSize, 'employeeBeesNeighborSize':_emplNeig, 'onLookerBeesSize':_onlookSize, 'onLookerBeesNeighborSize':_onLookNeig, 'scoutBeesSize':_scoutSize, 'maxCicles':_cicles, 'onLookerBeesFavouredSolutionsNumber':_onLookerFavSolutionNmb}
        print('##### PARAMS #####')
        print(PARAMS_ABC)
        print('##################')
        print('##### MAKE REQUEST - ABC #####')
        r = requests.get(url = ABC_URL, params = PARAMS_ABC, verify=False)
        print('##### RESPONSE #####')
        j = r.json()
        a_set = set(j['coloredNodes'])
        array[idx] += len(a_set)             
        print(len(a_set))
    print("***************** END ABC ALGORITHM *****************")    

emplBeeSizes = [5,10,15,20,25]
emplBeeNeigh = [1,3,6,9,12]    
onLookerbees = [5,10,15,20,25]
onLookerNeig = [1,3,6,9,12]
onLookerFavSol = [1,2,3,4,5]
scoutBeesSiz = [5,10,15,20,25]
maxCicles = [10,20,30,40,50]

print("**********************   START   *********************************")

ids = [29, 32, 33]

for idd in ids:
    performGreedyTests(idd, 5)
    performLargestFirst(idd)

    array = [0,0,0,0,0]
    for idx, ms in enumerate(maxCicles):
        print(f"************ TESTING LOOP for {ms} - Cicles")
        performABC(idd, 5, emplBeeSizes[2], emplBeeNeigh[2], onLookerbees[2], onLookerNeig[2], scoutBeesSiz[2], ms, onLookerFavSol[2],idx)
        print(array)
        print(f"*****************************************************")
    for i in range(5):
        array[i] = array[i]/5
    min_value = min(array)
    m_i_ms = array.index(min_value)

    print(f"@@@@@@@@@@@@@@@@@@@@@@@@@ {m_i_ms}")

    array = [0,0,0,0,0]
    for idx, eBS in enumerate(emplBeeSizes):
        print(f"************ TESTING LOOP for {eBS} - EmployeeBeeSize")
        performABC(idd, 5, eBS, emplBeeNeigh[2], onLookerbees[2], onLookerNeig[2], scoutBeesSiz[2], maxCicles[m_i_ms], onLookerFavSol[2],idx)
        print(array)
        print(f"*****************************************************")
    for i in range(5):
        array[i] = array[i]/5
    min_value = min(array)
    m_i_emplBS = array.index(min_value)

    print(f"@@@@@@@@@@@@@@@@@@@@@@@@@ {m_i_emplBS}")

    array = [0,0,0,0,0]
    for idx, eBN in enumerate(emplBeeNeigh):
        print(f"************ TESTING LOOP for {eBN} - EmployeeBeeNeighbor")
        performABC(idd, 5, emplBeeSizes[m_i_emplBS], eBN, onLookerbees[2], onLookerNeig[2], scoutBeesSiz[2], maxCicles[m_i_ms], onLookerFavSol[2],idx)
        print(array)
        print(f"*****************************************************")
    for i in range(5):
        array[i] = array[i]/5
    min_value = min(array)
    m_i_emplBN = array.index(min_value)

    print(f"@@@@@@@@@@@@@@@@@@@@@@@@@ {m_i_emplBN}")


    array = [0,0,0,0,0]
    for idx, olkBS in enumerate(onLookerbees):
        print(f"************ TESTING LOOP for {olkBS} - OnLookerBeesSize")
        performABC(idd, 5, emplBeeSizes[m_i_emplBS], emplBeeNeigh[m_i_emplBN], olkBS, onLookerNeig[2], scoutBeesSiz[2], maxCicles[m_i_ms], onLookerFavSol[2],idx)
        print(array)
        print(f"*****************************************************")
    for i in range(5):
        array[i] = array[i]/5
    min_value = min(array)
    m_i_olkBS = array.index(min_value)

    print(f"@@@@@@@@@@@@@@@@@@@@@@@@@ {m_i_olkBS}")


    array = [0,0,0,0,0]
    for idx, olkFS in enumerate(onLookerFavSol):
        print(f"************ TESTING LOOP for {olkFS} - OnLookerFavouredSolutions")
        performABC(idd, 5, emplBeeSizes[m_i_emplBS], emplBeeNeigh[m_i_emplBN], onLookerbees[m_i_olkBS], onLookerNeig[2], scoutBeesSiz[2], maxCicles[m_i_ms], olkFS,idx)
        print(array)
        print(f"*****************************************************")
    for i in range(5):
        array[i] = array[i]/5
    min_value = min(array)
    m_i_olkFS = array.index(min_value)

    print(f"@@@@@@@@@@@@@@@@@@@@@@@@@ {m_i_olkFS}")


    array = [0,0,0,0,0]
    for idx, olkN in enumerate(onLookerNeig):
        print(f"************ TESTING LOOP for {olkN} - OnLookerNeighbour")
        performABC(idd, 5, emplBeeSizes[m_i_emplBS], emplBeeNeigh[m_i_emplBN], onLookerbees[m_i_olkBS], olkN, scoutBeesSiz[2], maxCicles[m_i_ms], onLookerFavSol[m_i_olkFS],idx)
        print(array)
        print(f"*****************************************************")
    for i in range(5):
        array[i] = array[i]/5
    min_value = min(array)
    m_i_olkN = array.index(min_value)

    print(f"@@@@@@@@@@@@@@@@@@@@@@@@@ {m_i_olkN}")

    array = [0,0,0,0,0]
    for idx, stcBS in enumerate(scoutBeesSiz):
        print(f"************ TESTING LOOP for {olkN} - ScoutBeesSize")
        performABC(idd, 5, emplBeeSizes[m_i_emplBS], emplBeeNeigh[m_i_emplBN], onLookerbees[m_i_olkBS], onLookerNeig[m_i_olkN], stcBS, maxCicles[m_i_ms], onLookerFavSol[m_i_olkFS],idx)
        print(array)
        print(f"*****************************************************")
    for i in range(5):
        array[i] = array[i]/5
    min_value = min(array)
    m_i_stcBS = array.index(min_value)

    print(f"@@@@@@@@@@@@@@@@@@@@@@@@@ {m_i_stcBS}")

    print(f"{m_i_ms},{m_i_emplBS},{m_i_emplBN},{m_i_olkBS},{m_i_olkFS},{m_i_olkN},{m_i_stcBS}")