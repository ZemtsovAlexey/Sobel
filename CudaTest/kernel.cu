#include "cuda_runtime.h"
#include "device_launch_parameters.h"

#include <stdio.h>

#include <chrono>
//#include <iostream>
//void addWithCuda(int *c, const int *a, const int *b, unsigned int size);
__global__ void addKernel(int *c, const int *a, const int *b)
{
	//c[threadIdx.x] = a[threadIdx.x] + b[threadIdx.x];

	unsigned int tid = blockIdx.x * blockDim.x + threadIdx.x;
	const int n = sizeof c;

	while (tid < n)
	{
		c[tid] = a[tid] + b[tid];
		tid += blockDim.x * gridDim.x;
	}
}

__global__ void add_threads(double *C, double* A, int inLength, int outLength) {

	unsigned int tid = blockIdx.x * blockDim.x + threadIdx.x;
	//const int n = sizeof Z * X * Y;
	//unsigned int rid = tid % (X * Y);

	/*char colText[] = "col - X\n";
	colText[6] = rid + '0';
	printf(colText);*/

	if (tid < inLength)
	{
		int rid = tid % outLength;


		char colText[] = "col - X Y\n";
		colText[6] = C[tid] + '0';
		colText[8] = A[rid] + '0';
		printf(colText);

		A[rid] = A[rid] + C[tid];

	}

	/*if (tid < inLength)
	{
		double sum = 0;
		int rid = tid % outLength;
		
		A[rid] += C[tid];
	}*/

}

__global__
void add(double *x, double *y, int n, int o)
{
	int index = blockIdx.x * blockDim.x + threadIdx.x;
	int stride = blockDim.x * gridDim.x;
	
	for (int i = index; i < n; i += stride)
	{
		y[i % o] = y[i % o] + x[i];
	}
}

__global__ void matrixSumKernel(double *A, const double *C, int X, int Y, int Z) {

	int ROW = blockIdx.y*blockDim.y + threadIdx.y;
	int COL = blockIdx.x*blockDim.x + threadIdx.x;
	int z = blockIdx.z*blockDim.z + threadIdx.z;

	A[ROW * X + COL] += C[(z * X * Y) + (ROW * X + COL)];

	/*if (ROW < Y && COL < X) {
		for (int i = 0; i < Z; i++) {
			A[ROW * X + COL] += C[(i * X * Y) + (ROW * X + COL)];
		}
	}*/
}
//const int N = 16;
//__global__ void MatAdd(float A[N][N], float B[N][N], float C[N][N]) {
//	int i = blockIdx.x * blockDim.x + threadIdx.x;
//	int j = blockIdx.y * blockDim.y + threadIdx.y;
//
//	if (i < N && j < N)
//		C[i][j] = A[i][j] + B[i][j];
//}

// Helper function for using CUDA to add vectors in parallel.
extern "C" __declspec(dllexport) void addWithCuda(int *c, const int *a, const int *b, unsigned int N)
{
	//Device array
	int *dev_a, *dev_b, *dev_c;

	//Allocate the memory on the GPU
	cudaMalloc((void **)&dev_a, N * sizeof(int));
	cudaMalloc((void **)&dev_b, N * sizeof(int));
	cudaMalloc((void **)&dev_c, N * sizeof(int));

	//Copy Host array to Device array
	cudaMemcpy(dev_a, a, N * sizeof(int), cudaMemcpyHostToDevice);
	cudaMemcpy(dev_b, b, N * sizeof(int), cudaMemcpyHostToDevice);

	//Make a call to GPU kernel
	//addKernel <<< (N + 511) / 512, 512 >>> (dev_a, dev_b, dev_c);
	addKernel << < (N + 511) / 512, 512 >> > (dev_c, dev_a, dev_b);

	//Copy back to Host array from Device array
	cudaMemcpy(c, dev_c, N * sizeof(int), cudaMemcpyDeviceToHost);

	//Display the result
	/*for (int i = 0; i<N; i++)
	printf("%d + %d = %d\n", a[i], b[i], c[i]);*/

	//Free the Device array memory
	cudaFree(dev_a);
	cudaFree(dev_b);
	cudaFree(dev_c);
}

extern "C" __declspec(dllexport) void matrixSum(double b[], const double a[], int x, int y, int z)
{
	//Device array
	double *dev_a, *dev_b;
	unsigned int N = z * y * x;
	int outLength = y * x;

	//Allocate the memory on the GPU
	cudaMalloc((void **)&dev_a, z * x * y * sizeof(double));
	cudaMalloc((void **)&dev_b, x * y * sizeof(double));

	//Copy Host array to Device array
	cudaMemcpy(dev_a, a, z * x * y * sizeof(double), cudaMemcpyHostToDevice);
	cudaMemcpy(dev_b, b, x * y * sizeof(double), cudaMemcpyHostToDevice);

	//Make a call to GPU kernel
	int blockSize = 256;
	int numBlocks = (N + blockSize - 1) / blockSize;
	add << < numBlocks, blockSize >> > (dev_a, dev_b, N, outLength);
	//MatAdd << < numBlocks, threadsPerBlock >> > (a, b,);

	cudaThreadSynchronize();

	//Copy back to Host array from Device array
	cudaMemcpy(b, dev_b, x * y * sizeof(double), cudaMemcpyDeviceToHost);
	
	//Free the Device array memory
	cudaFree(dev_a);
	cudaFree(dev_b);
}
