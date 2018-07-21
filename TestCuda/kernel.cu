
#include "cuda_runtime.h"
#include "device_launch_parameters.h"

#include <cuda.h>
#include <stdio.h>
#include <math.h>

#include <assert.h>
#include <cstdlib>

struct Test
{
	double** Cord;
};

__global__ void kernel(int a, int b, int *c)
{
	*c = (a + b)*(a + b);
}

__global__ void VecAdd(const double* A, const double* B, double *C, int N)
{
	int i = blockDim.x * blockIdx.x + threadIdx.x;
	if (i < N)
		*C += A[i] * B[i];
}

__global__ void SumMatrix(double *A, double* B, int length0, int length1, int length2)
{
	/*const int N = 30000;
	int idx = blockDim.x * blockIdx.x + threadIdx.x;*/
	const int blockSize = length1 * length2;
	//printf(" %d \n", 2);

	//printf("blockSize \r\n");

	//if (idx < N) {
		for (int i = 0; i < length0; i++) {
			for (int y = 0; y < length1; y++) {
				for (int x = 0; x < length2; x++) {
					//*A[y * length2 + x] += B[(i * blockSize) + (y * length2 + x)];
					A[y * length2 + x] += B[(i * blockSize) + (y * length2 + x)];
				}
			}
		}
	//}
}

__global__ void matrixMultiplicationKernel(double* A, double* B, double* C, int N) {

	int ROW = blockIdx.y*blockDim.y + threadIdx.y;
	int COL = blockIdx.x*blockDim.x + threadIdx.x;

	float tmpSum = 0;

	if (ROW < N && COL < N) {
		C[ROW * N + COL] = A[ROW * N + COL] + B[ROW * N + COL];

		/*char rowText[] = "row - X\n";
		rowText[6] = ROW + '0';
		printf(rowText);

		char colText[] = "col - X\n";
		colText[6] = COL + '0';
		printf(colText);

		char valText[] = "val - X\n";
		valText[6] = A[ROW * N + COL] + '0';
		printf(valText);*/
		// each thread computes one element of the block sub-matrix
		/*for (int i = 0; i < N; i++) {
		tmpSum += A[ROW * N + i] * B[i * N + COL];
		}*/
	}
	//C[ROW * N + COL] = tmpSum;
}

__global__ void matrixSum(double* A, double* C, int X, int Y, int Z) {

	int ROW = blockIdx.y*blockDim.y + threadIdx.y;
	int COL = blockIdx.x*blockDim.x + threadIdx.x;

	//float tmpSum = 0;

	if (ROW < Y && COL < X) {
		for (int i = 0; i < Z; i++) {
			C[ROW * X + COL] += A[(ROW * Y * X) + (ROW * X + COL)];
		}
		//tmpSum += A[ROW * N + COL];
	}
	//C[ROW * X + COL] = tmpSum;
}

__global__ void add_threads(double *a, double *b, int z) {

	/* threadIdx.x gives the thread ID in each block */

	/*char rowText[] = "row - X\n";
	rowText[6] = blockIdx.x + '0';
	printf(rowText);*/

	for (unsigned int i = 0; i < z; i++)
		b[blockIdx.x] += (2 / (1 + exp(-2 * a[blockIdx.x]))) - 1;

}

#define N 1000

__global__
void add(int *a, int *b) {
	int i = blockIdx.x;
	if (i<N) {
		b[i] = 2 * a[i];
	}
}

int main()
{
	////
	//// Create int arrays on the CPU.
	//// ('h' stands for "host".)
	////
	//int ha[N], hb[N];

	////
	//// Create corresponding int arrays on the GPU.
	//// ('d' stands for "device".)
	////
	//int *da, *db;
	//cudaMalloc((void **)&da, N * sizeof(int));
	//cudaMalloc((void **)&db, N * sizeof(int));

	////
	//// Initialise the input data on the CPU.
	////
	//for (int i = 0; i<N; ++i) {
	//	ha[i] = i;
	//}

	////
	//// Copy input data to array on GPU.
	////
	//cudaMemcpy(da, ha, N * sizeof(int), cudaMemcpyHostToDevice);

	////
	//// Launch GPU code with N threads, one per
	//// array element.
	////
	//add<<<N, 1 >>>(da, db);

	////
	//// Copy output array from GPU back to CPU.
	////
	//cudaMemcpy(hb, db, N * sizeof(int), cudaMemcpyDeviceToHost);

	//for (int i = 0; i<N; ++i) {
	//	printf("%d\n", hb[i]);
	//}

	////
	//// Free up the arrays on the GPU.
	////
	//cudaFree(da);
	//cudaFree(db);

	return 0;
}