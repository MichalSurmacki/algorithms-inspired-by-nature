using GraphColoring.Application.Dtos.Graphs;
using GraphColoring.Application.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GraphColoring.Application.Algorithms.ABC
{
    public class ABC
    {
        public GraphReadDto BestResult { get; private set; }
        
        private readonly object _locker = new object();
        private Random rnd = new Random();
        
        private List<EmployeeBee> _employedBees = new List<EmployeeBee>();
        private List<OnLookerBee> _onLookerBees = new List<OnLookerBee>();
        private List<ScoutBee> _scoutBees = new List<ScoutBee>();
        private readonly GraphReadDto _rawGraph;
        private readonly int _maxCicles;
        private readonly int _onlkFavouredSolutionsNmb;
        private readonly int _emplNeighSize;

        public ABC(GraphReadDto graph, int emplBeesSize, int emplNeighLookNmb, int onlkBeesSize, int onlkNeighLookNmb, int sctBeesSize, int maxCicles, int onlkChunkNmb)
        {
            _rawGraph = graph;
            _maxCicles = maxCicles;
            _onlkFavouredSolutionsNmb = onlkChunkNmb;
            _emplNeighSize = emplNeighLookNmb;
            InitEmployeeBees(emplBeesSize, emplNeighLookNmb);
            InitOnLookerBees(onlkBeesSize, onlkNeighLookNmb);
            InitScoutBees(sctBeesSize);
        }

        public async Task<GraphReadDto> Start()
        {
            //GŁÓWNA PĘTLA ALGORYTMU
            int x = 0;
            for (int i = 0; i< _maxCicles; i++)
            {
                int actualBest = BestResult.NumberOfColorsInGraph;
                string logInfo = $"{i} z {_maxCicles} | {actualBest} |";
                await EmployeeBeesPart(logInfo);
                await OnLookerBeesPart(logInfo);
                await ScoutBeesPart(logInfo);
            }
            return BestResult;
        }

        private void InitEmployeeBees(int beesSize, int neighborhoodSize)
        {
            for (int i = 0; i < beesSize; i++)
            {
                // znalezienie rozwiazania poczatkowego dla poszczególnych pszczół robotnic
                var initialSolution = _rawGraph.MakeACopy();
                Greedy.Start(ref initialSolution);
                // znalezienie najlepszego rozwiazania dotychczas
                if (BestResult == null || BestResult.NumberOfColorsInGraph > initialSolution.NumberOfColorsInGraph)
                {
                    BestResult = initialSolution.MakeACopy();
                }
                var chance = (float)rnd.NextDouble();
                _employedBees.Add(new EmployeeBee(initialSolution, neighborhoodSize, chance, $"E{i}"));
            }
        }

        private void InitOnLookerBees(int beesSize, int neighborhoodSize)
        {
            for (int i = 0; i < beesSize; i++)
            {
                _onLookerBees.Add(new OnLookerBee(neighborhoodSize, $"O{i}"));
            }
        }

        private void InitScoutBees(int beesSize)
        {
            for (int i = 0; i < beesSize; i++)
            {
                _scoutBees.Add(new ScoutBee(_rawGraph.MakeACopy(), $"S{i}"));
            }
        }

        private async Task<bool> EmployeeBeesPart(string logInfo)
        {
            // wypuszczenie pszczół w poszukiwaniu rozwiązań
            for (int j = 0; j < _employedBees.Count; j++)
            {
                _employedBees[j].SetTaskAction(logInfo);
                _employedBees[j].TaskAction.Start();
            }
            // czekanie aż skończą przeszukiwać rozwiązania
            await Task.WhenAll(_employedBees.Select(e => e.TaskAction).ToList());
            // sprawdzenie czy któraś z pszczół robotnic stanie się skautem
            for (int j = _employedBees.Count - 1; j >= 0; j--)
            {
                if (_employedBees[j].IsChangeNecessary)
                {
                    _scoutBees.Add(new ScoutBee(_rawGraph.MakeACopy(), _employedBees[j].Id));
                    _employedBees.RemoveAt(j);
                }
            }
            // sprawdzenie czy któreś rozwiązanie znalezione przez robotnice jest najlepsze
            var potentialBest = _employedBees.Select(e => e.BestSolution).ToList()
                                    .OrderByDescending(e => e.ColorClassesCount.Count).FirstOrDefault();
            if (potentialBest != null && potentialBest.NumberOfColorsInGraph < BestResult.NumberOfColorsInGraph)
            {
                BestResult = potentialBest.MakeACopy();
            }
            return true;
        }

        private async Task<bool> OnLookerBeesPart(string logInfo)
        {
            // jeśli wszystkie pszczoły employee przeistoczyły się w scouty
            // to zaprzęgnięcie OnLooker do pracy w sąsiedztwie najlepszego rozwiązania
            if (_employedBees.Count == 0)
            {
                foreach (var onlkBee in _onLookerBees)
                {
                    onlkBee.SetBestAndInitSolutions(BestResult.MakeACopy(), BestResult.MakeACopy());
                    onlkBee.SetTaskAction(logInfo);
                    onlkBee.TaskAction.Start();
                }
                await Task.WhenAll(_onLookerBees.Select(x => x.TaskAction).ToList());
                var potentialBestSolution = _onLookerBees.Select(e => e.BestSolution).ToList()
                                    .OrderByDescending(e => e.ColorClassesCount.Count).FirstOrDefault();
                if (potentialBestSolution.NumberOfColorsInGraph < BestResult.NumberOfColorsInGraph)
                {
                    BestResult = potentialBestSolution.MakeACopy();
                }
                return true;
            }

            // podzielenie pszczół zgodnie z ustawieniami
            List<List<OnLookerBee>> onLookerBeesChunks;
            if (_onlkFavouredSolutionsNmb > 0 && _onlkFavouredSolutionsNmb != 1)
            {
                onLookerBeesChunks = _onLookerBees.Split(_onlkFavouredSolutionsNmb);
            }
            else
            {
                onLookerBeesChunks = new List<List<OnLookerBee>>();
                onLookerBeesChunks.Add(_onLookerBees);
            }
            // sprawdzenie czy liczba onlooker nie jest większa niż liczba employee
            if (onLookerBeesChunks.Count > _employedBees.Count)
            {
                onLookerBeesChunks = _onLookerBees.Split(_employedBees.Count);
            }
            // posotrowanie najlepszych rozwiazań w celu intensyfikacji poszukiwań przez OnLookerBee
            _employedBees = _employedBees.OrderByDescending(e => e.OverallScore).ToList();
            for (int j = 0; j < onLookerBeesChunks.Count; j++)
            {
                foreach (var onlkBee in onLookerBeesChunks[j])
                {
                    onlkBee.SetBestAndInitSolutions(BestResult.MakeACopy(), _employedBees[j].BestSolution.MakeACopy());
                    onlkBee.SetTaskAction(logInfo);
                    onlkBee.TaskAction.Start();
                }
            }
            var tt = onLookerBeesChunks.SelectMany(x => x).ToList().Select(x => x.TaskAction).ToList();
            await Task.WhenAll(tt);
            // sprawdzenie czy któreś rozwiązanie znalezione przez onLookerBees jest najlepsze
            var potentialBest = onLookerBeesChunks.SelectMany(x => x).Select(e => e.BestSolution).ToList()
                                    .OrderByDescending(e => e.ColorClassesCount.Count).FirstOrDefault();
            if (potentialBest.NumberOfColorsInGraph < BestResult.NumberOfColorsInGraph)
            {
                BestResult = potentialBest.MakeACopy();
            }
            return true;
        }

        private async Task<bool> ScoutBeesPart(string logInfo)
        {
            foreach (ScoutBee b in _scoutBees)
            {
                b.SetTaskAction(logInfo);
                b.TaskAction.Start();
            }
            await Task.WhenAll(_scoutBees.Select(e => e.TaskAction).ToList());
            // if scout.best > overallbest -> scout zmienia sie w employee
            // sprawdzenie czy któraś z pszczół zwiadowców stanie się robotnicą
            for (int j = _scoutBees.Count - 1; j >= 0; j--)
            {
                var chance = (float)rnd.NextDouble();
                // jeśli zwiadowca znalazł lepsze rozwiązanie staje się robotnicą - ustawienie BestResult
                if (_scoutBees[j].FoundSolution.NumberOfColorsInGraph < BestResult.NumberOfColorsInGraph)
                {
                    _employedBees.Add(new EmployeeBee(_scoutBees[j].FoundSolution.MakeACopy(), _emplNeighSize, chance, _scoutBees[j].Id));
                    BestResult = _scoutBees[j].FoundSolution.MakeACopy();
                    _scoutBees.RemoveAt(j);
                }
                // jeśli zwiadowca znalazł rozwiązanie o takiej samej liczbie kolorów, również staje się robotnicą (dajemy szanse na przeszukanie sąsiedztwa)
                else if (_scoutBees[j].FoundSolution.NumberOfColorsInGraph == BestResult.NumberOfColorsInGraph)
                {
                    _employedBees.Add(new EmployeeBee(_scoutBees[j].FoundSolution.MakeACopy(), _emplNeighSize, chance, _scoutBees[j].Id));
                    _scoutBees.RemoveAt(j);
                }
            }
            return true;
        }
    }
}
