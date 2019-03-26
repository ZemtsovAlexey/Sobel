#include <cuda.h>
#include <cuda_runtime_api.h>
#include "device_launch_parameters.h"
#include <stdio.h>
#include <stdlib.h>

struct GridStruct
{
	dim3 gridDim;
	dim3 threadsPerBlock;
};

__global__ void rot90(double* input, double* output, int width, int height)
{
	int x = threadIdx.x + blockIdx.x * blockDim.x;
	int y = threadIdx.y + blockIdx.y * blockDim.y;

	if (y < height && x < width)
		output[x * width + (height - 1 - y)] = input[y * width + x];
}

__global__ void rot180(double* input, double* output, int width, int height)
{
	int x = threadIdx.x + blockIdx.x * blockDim.x;
	int y = threadIdx.y + blockIdx.y * blockDim.y;

	if (y < height && x < width)
	{
		output[(height - 1 - y) * width + (width - 1 - x)] = input[y * width + x];
	}
}

__global__ void Multiply(double* result, double* a, double* b)
{
	int x = threadIdx.x + blockIdx.x * blockDim.x;

	result[x] = a[x] * b[x];
}

__global__ void Multiply2(double* result, double* input, double* weights, int len)
{
	int x = threadIdx.x + blockIdx.x * blockDim.x;
	int i = x >= len ? x % len : x;

	if (i < len) {
		result[x] = input[i] * weights[x];
	}
}

__global__ void convolution(
	double* output,
	double* input,
	double* kernel,
	int inputWidth,
	int inputHeight,
	int kernelWidth,
	int kernelHeight,
	int outWidth,
	int outHeight
)
{
	//extern __shared__ double sh[];
	__shared__ double sum;

	int x = threadIdx.x + blockIdx.x;// *blockDim.x;
	int y = threadIdx.y + blockIdx.y;// *blockDim.y;

	if (x < inputWidth && y < inputHeight)
	{
		//double sum = 0;
		/*for (int ky = 0; ky < kernelHeight; ky++)
		{
			for (int kx = 0; kx < kernelHeight; kx++)
			{
				sum += (input[(y + ky) * inputWidth + (x + kx)] * kernel[ky * kernelWidth + kx]);
			}
		}*/

		sum = sum + (input[(y)* inputWidth + (x)] * kernel[threadIdx.y * kernelWidth + threadIdx.x]);

		__syncthreads();

		output[blockIdx.y * outWidth + blockIdx.x] = sum;
		sum = 0;
	}
}

//__global__ void ArraySum(float *array)
//
//{
//
//	int index = threadIdx.x;
//
//	sum = sum + array[index];
//
//	__syncthreads();
//
//}

__global__ void calcSum(
	double output, 
	double* input, 
	double* kernel, 
	int inputWidth, 
	int inputHeight,
	int kernelWidth,
	int kernelHeight
)
{
	extern __shared__ double sum;

	int x = threadIdx.x + blockIdx.x * blockDim.x;
	int y = threadIdx.y + blockIdx.y * blockDim.y;

	if (x < inputWidth && y < inputHeight)
	{
		sum += (input[(y)* inputWidth + (x)] * kernel[threadIdx.y * kernelWidth + threadIdx.x]);

		__syncthreads();

		output = sum;
	}
}

//__global__
//void transpose_2(double* a, double* b, int N)
//{
//	__shared__ double sh[8][8];
//
//	int x = blockIdx.x * blockDim.x;
//	int y = blockIdx.y * blockDim.y;
//	int i = x + threadIdx.x;
//	int j = y + threadIdx.y;
//
//	sh[threadIdx.y][threadIdx.x] = a[j * N + i];
//
//	__syncthreads();
//
//	b[((x + threadIdx.x)) * N + (N - 1 - (y + threadIdx.y))] = sh[threadIdx.y][threadIdx.x];
//}

GridStruct getGridModel(int width, int length)
{
	GridStruct result;

	int height = length / width;
	int BSX = width > 127 ? 127 : width;
	int BSY = height > 127 ? 127 : height;

	int vx = width % BSX > 0 ? (width / BSX) + 1 : width / BSX;
	int vy = height % BSY > 0 ? (height / BSY) + 1 : height / BSY;

	result.gridDim = dim3(vx, vy, 1);
	result.threadsPerBlock = dim3(BSX, BSY, 1);

	return result;
}

extern "C" __declspec(dllexport) void Rot90GPU(double output[], double input[], int width, int length)
{
	double *dev_a, *dev_b;
	int height = length / width;

	cudaMalloc(&dev_a, sizeof(double) * width * height);
	cudaMalloc(&dev_b, sizeof(double) * width * height);
	cudaMemcpy(dev_a, input, sizeof(double) * width * height, cudaMemcpyHostToDevice);

	GridStruct grid = getGridModel(width, length);

	rot90<<<grid.gridDim, grid.threadsPerBlock>>>(dev_a, dev_b, width, height);

	cudaDeviceSynchronize();

	cudaMemcpy(output, dev_b, sizeof(double) * width * height, cudaMemcpyDeviceToHost);

	cudaFree(dev_a);
	cudaFree(dev_b);
}

extern "C" __declspec(dllexport) void Rot180GPU(double output[], double input[], int width, int length)
{
	double *dev_a, *dev_b;
	int height = length / width;

	cudaMallocHost(&dev_a, sizeof(double) * width * height);
	cudaMallocHost(&dev_b, sizeof(double) * width * height);
	cudaMemcpy(dev_a, input, sizeof(double) * width * height, cudaMemcpyHostToHost);

	GridStruct grid = getGridModel(width, length);

	rot180 << <grid.gridDim, grid.threadsPerBlock >> > (dev_a, dev_b, width, height);

	cudaDeviceSynchronize();

	cudaMemcpy(output, dev_b, sizeof(double) * width * height, cudaMemcpyHostToHost);

	cudaFree(dev_a);
	cudaFree(dev_b);
}

extern "C" __declspec(dllexport) void MultiplyGPU(double output[], double input[], double weights[], int len)
{
	double *dev_output, *dev_a, *dev_b;

	cudaMalloc(&dev_output, sizeof(double) * len);
	cudaMalloc(&dev_a, sizeof(double) * len);
	cudaMalloc(&dev_b, sizeof(double) * len);
	cudaMemcpy(dev_a, input, sizeof(double) * len, cudaMemcpyHostToDevice);
	cudaMemcpy(dev_b, weights, sizeof(double) * len, cudaMemcpyHostToDevice);

	const int size = 511;
	int threads = len > size ? size : len;
	int blocks = len % size > 0 ? len / threads + 1 : len / threads;

	Multiply << <blocks, threads >> > (dev_output, dev_a, dev_b);

	cudaDeviceSynchronize();

	cudaMemcpy(output, dev_output, sizeof(double) * len, cudaMemcpyDeviceToHost);

	cudaFree(dev_output);
	cudaFree(dev_a);
	cudaFree(dev_b);
}

extern "C" __declspec(dllexport) void Multiply2GPU(double output[], double input[], double weights[], int len, int wlen, int nlen)
{
	double *dev_output, *dev_input, *dev_weights;

	cudaMalloc(&dev_output, sizeof(double) * wlen);
	cudaMalloc(&dev_input, sizeof(double) * len);
	cudaMalloc(&dev_weights, sizeof(double) * wlen);
	cudaMemcpy(dev_input, input, sizeof(double) * len, cudaMemcpyHostToDevice);
	cudaMemcpy(dev_weights, weights, sizeof(double) * wlen, cudaMemcpyHostToDevice);

	const int size = 500;
	int threads = wlen > size ? size : wlen;
	int blocks = wlen % size > 0 ? wlen / threads + 1 : wlen / threads;

	Multiply2 << <blocks, threads >> > (dev_output, dev_input, dev_weights, len);

	cudaDeviceSynchronize();

	double *o = new double[wlen];

	cudaMemcpy(o, dev_output, sizeof(double) * wlen, cudaMemcpyDeviceToHost);

	for (int i = 0; i < wlen; i++) 
	{
		output[i / wlen] += *(o + i);
	}

	cudaFree(dev_output);
	cudaFree(dev_input);
	cudaFree(dev_weights);
}

extern "C" __declspec(dllexport) void ConvolutionGPU(
	double output[],
	double input[],
	double kernel[],
	int inputWidth,
	int inputHeight,
	int kernelWidth,
	int kernelHeight,
	int outWidth,
	int outHeight
)
{
	double *dev_input, *dev_kernel, *dev_output;
	
	int BSX = kernelWidth;// > 512 ? 512 : kernelWidth;
	int BSY = kernelHeight;// > 512 ? 512 : kernelHeight;

	/*int vx = inputWidth % BSX > 0 ? (inputWidth / BSX) + 1 : inputWidth / BSX;
	int vy = inputHeight % BSY > 0 ? (inputHeight / BSY) + 1 : inputHeight / BSY;*/

	dim3 gridDim = dim3(outWidth, outHeight, 1);
	dim3 threadsPerBlock = dim3(kernelWidth, kernelHeight, 1);

	cudaMalloc(&dev_input, sizeof(double) * inputWidth * inputHeight);
	cudaMalloc(&dev_kernel, sizeof(double) * kernelWidth * kernelHeight);
	cudaMalloc(&dev_output, sizeof(double) * outWidth * outHeight);
	cudaMemcpy(dev_input, input, sizeof(double) * inputWidth * inputHeight, cudaMemcpyHostToDevice);
	cudaMemcpy(dev_kernel, kernel, sizeof(double) * kernelWidth * kernelHeight, cudaMemcpyHostToDevice);

	convolution<<<gridDim, threadsPerBlock>>>(dev_output, dev_input, dev_kernel, inputWidth, inputHeight, kernelWidth, kernelHeight, outWidth, outHeight);

	cudaDeviceSynchronize();

	cudaMemcpy(output, dev_output, sizeof(double) * outWidth * outHeight, cudaMemcpyDeviceToHost);

	cudaFree(dev_input);
	cudaFree(dev_kernel);
	cudaFree(dev_output);
}