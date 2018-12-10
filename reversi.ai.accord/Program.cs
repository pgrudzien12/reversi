using Accord.Genetic;
using Accord.Neuro;
using Accord.Neuro.Learning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace reversi.ai
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            bool needToStop = false;
            var ancestor = new Mind();
            var fitness = new ReversiPopulation();
            // generate population of size n
            Population population = new ReversiPopulation();
            
            while (!needToStop)
            {
                population.RunEpoch();
                // play each against each
                // assign values to chormosomes
                // choose best chormosomes
                // save results
                // mutate
            }
        }
    }

    public class ReversiPopulation : Population, IFitnessFunction
    {
        public ReversiPopulation()
            : base(30, new Mind(), new DullFunction(), new EliteSelection())
        {
            FitnessFunction = this;
        }

        public override void Selection()
        {
            for (int i = 0; i < base.Size; i++)
            {
                for (int j = 0; j < base.Size; j++)
                {
                    if (i == j)
                        continue;
                    var a = base[i] as Mind;
                    var b = base[j] as Mind;
                    Game reversiGame = new Game(a, b);
                    reversiGame.PlayAsync().Wait();
                    var winner = reversiGame.Board.currStatus.currTurn;
                    if (winner == Piece.Red)
                        a.Points++;
                    else
                        b.Points++;
                }
            }
            base.Selection();
        }

        public double Evaluate(IChromosome chromosome)
        {
            Mind m = chromosome as Mind;
            
            return m.Points;
        }
    }

    internal class DullFunction : IFitnessFunction
    {
        public double Evaluate(IChromosome chromosome)
        {
            return 0;
        }
    }

    public class Mind : ShortArrayChromosome, IPlayerController
    {
        private static int chromosomeLength = 64 * 16 + 16;
        ActivationNetwork network;

        public int Points { get; set; }

        public Mind()
            : base(chromosomeLength)
        {
            network = CreateNetwork(Value);
        }

        protected Mind(Mind source)
            : base(source)
        {
            network = CreateNetwork(source.Value);
        }

        private ActivationNetwork CreateNetwork(ushort[] values)
        {
            ActivationNetwork network = new ActivationNetwork(
                            new RectifiedLinearFunction(),
                            64, // two inputs in the network
                            16, // two neurons in the first layer
                            1); // one neuron in the second layer
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    network.Layers[0].Neurons[i].Weights[j] = (double)Value[i * j] / maxValue;
                }
            }
            for (int i = 0; i < 16; i++)
            {
                network.Layers[1].Neurons[0].Weights[i] = ((double)Value[64 + 16 + i] / maxValue);
            }
            return network;
        }

        public override IChromosome Clone()
        {
            var chromosome = new Mind(this);
            return chromosome;
        }

        public override IChromosome CreateNew()
        {
            return new Mind();
        }



        public Task<MoveDescriptor> MakeMove(Board board, CancellationToken cancellationToken)
        {
            var validMoves = board.ValidMoves(board.currStatus.currTurn);
            var best = 0;
            var bestValue = -1000;
            for (int i = 0; i < validMoves.Length; i++)
            {
                double[] b = AsDoubles(board);
                var result = network.Compute(b)[0];
                if (result >= bestValue)
                    best = i;
            }
            return Task.FromResult(validMoves[best]);
        }

        private double[] AsDoubles(Board b)
        {
            var board = new double[64];
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    switch (b[x, y])
                    {
                        case Piece.Blue:
                            board[x * y] = -1;
                            break;
                        case Piece.Red:
                            board[x * y] = 1;
                            break;
                        default:
                            board[x * y] = 0;
                            break;
                    }
                }
            }
            return board;
        }

        public Task OnMove(Board board, MoveDescriptor md)
        {
            return Task.CompletedTask;
        }
    }
}
