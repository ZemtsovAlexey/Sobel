/*
#include "cuda_runtime.h"
#include "device_launch_parameters.h"

#include <stdio.h>

cudaError_t addWithCuda(int *c, const int *a, const int *b, unsigned int size);

__global__ void addKernel(int *c, const int *a, const int *b)
{
    int i = threadIdx.x;
    c[i] = a[i] + b[i];
}

int main()
{
    const int arraySize = 5;
    const int a[arraySize] = { 1, 2, 3, 4, 5 };
    const int b[arraySize] = { 10, 20, 30, 40, 50 };
    int c[arraySize] = { 0 };

    // Add vectors in parallel.
    cudaError_t cudaStatus = addWithCuda(c, a, b, arraySize);
    if (cudaStatus != cudaSuccess) {
        fprintf(stderr, "addWithCuda failed!");
        return 1;
    }

    printf("{1,2,3,4,5} + {10,20,30,40,50} = {%d,%d,%d,%d,%d}\n",
        c[0], c[1], c[2], c[3], c[4]);

    // cudaDeviceReset must be called before exiting in order for profiling and
    // tracing tools such as Nsight and Visual Profiler to show complete traces.
    cudaStatus = cudaDeviceReset();
    if (cudaStatus != cudaSuccess) {
        fprintf(stderr, "cudaDeviceReset failed!");
        return 1;
    }

    return 0;
}

// Helper function for using CUDA to add vectors in parallel.
cudaError_t addWithCuda(int *c, const int *a, const int *b, unsigned int size)
{
    int *dev_a = 0;
    int *dev_b = 0;
    int *dev_c = 0;
    cudaError_t cudaStatus;

    // Choose which GPU to run on, change this on a multi-GPU system.
    cudaStatus = cudaSetDevice(0);
    if (cudaStatus != cudaSuccess) {
        fprintf(stderr, "cudaSetDevice failed!  Do you have a CUDA-capable GPU installed?");
        goto Error;
    }

    // Allocate GPU buffers for three vectors (two input, one output)    .
    cudaStatus = cudaMalloc((void**)&dev_c, size * sizeof(int));
    if (cudaStatus != cudaSuccess) {
        fprintf(stderr, "cudaMalloc failed!");
        goto Error;
    }

    cudaStatus = cudaMalloc((void**)&dev_a, size * sizeof(int));
    if (cudaStatus != cudaSuccess) {
        fprintf(stderr, "cudaMalloc failed!");
        goto Error;
    }

    cudaStatus = cudaMalloc((void**)&dev_b, size * sizeof(int));
    if (cudaStatus != cudaSuccess) {
        fprintf(stderr, "cudaMalloc failed!");
        goto Error;
    }

    // Copy input vectors from host memory to GPU buffers.
    cudaStatus = cudaMemcpy(dev_a, a, size * sizeof(int), cudaMemcpyHostToDevice);
    if (cudaStatus != cudaSuccess) {
        fprintf(stderr, "cudaMemcpy failed!");
        goto Error;
    }

    cudaStatus = cudaMemcpy(dev_b, b, size * sizeof(int), cudaMemcpyHostToDevice);
    if (cudaStatus != cudaSuccess) {
        fprintf(stderr, "cudaMemcpy failed!");
        goto Error;
    }

    // Launch a kernel on the GPU with one thread for each element.
    addKernel<<<1, size>>>(dev_c, dev_a, dev_b);

    // Check for any errors launching the kernel
    cudaStatus = cudaGetLastError();
    if (cudaStatus != cudaSuccess) {
        fprintf(stderr, "addKernel launch failed: %s\n", cudaGetErrorString(cudaStatus));
        goto Error;
    }
    
    // cudaDeviceSynchronize waits for the kernel to finish, and returns
    // any errors encountered during the launch.
    cudaStatus = cudaDeviceSynchronize();
    if (cudaStatus != cudaSuccess) {
        fprintf(stderr, "cudaDeviceSynchronize returned error code %d after launching addKernel!\n", cudaStatus);
        goto Error;
    }

    // Copy output vector from GPU buffer to host memory.
    cudaStatus = cudaMemcpy(c, dev_c, size * sizeof(int), cudaMemcpyDeviceToHost);
    if (cudaStatus != cudaSuccess) {
        fprintf(stderr, "cudaMemcpy failed!");
        goto Error;
    }

Error:
    cudaFree(dev_c);
    cudaFree(dev_a);
    cudaFree(dev_b);
    
    return cudaStatus;
}*/

#include "cuda_runtime.h"
#include "device_launch_parameters.h"
#include <stdio.h>
#include "sm_61_intrinsics.h"
#include "sm_60_atomic_functions.h"
#include "device_atomic_functions.h"
#include "device_double_functions.h"
#include "device_functions.h"

#if !defined(__CUDA_ARCH__) || __CUDA_ARCH__ >= 600
#else
__device__ double atomicAdd(double* address, double val)
{
	unsigned long long int* address_as_ull = (unsigned long long int*)address;
	unsigned long long int old = *address_as_ull, assumed;

	do {
		assumed = old;
		old = atomicCAS(address_as_ull, assumed, __double_as_longlong(val + __longlong_as_double(assumed)));
		// Note: uses integer comparison to avoid hang in case of NaN (since NaN != NaN)
	} while (assumed != old);

	//printf("%f\r\n", old);

	return __longlong_as_double(old);
}
#endif

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

template <typename T>
__global__ void rot180(const T* input, T* output, int width, int height)
{
	int x = threadIdx.x + blockDim.x * blockIdx.x;
	int y = threadIdx.y + blockDim.y * blockIdx.y;

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

__global__ void Multiply2(float* result, float* input, float* weights, int len)
{
	int x = threadIdx.x + blockIdx.x * blockDim.x;
	int i = x >= len ? x % len : x;

	result[x] = input[i] * weights[x];
}

__global__ void Multiply3(float* result, float* input, float* weights, int len, int neurons)
{
	int x = threadIdx.x;// +blockIdx.x * blockDim.x;

	if (x < neurons)
	{
		for (int i = 0; i < len; i++)
		{
			int n = neurons * x + i;
			result[x] += input[i] * weights[i];
		}
	}
}

const unsigned int MAX_FILTER_SIZE = 79;
__device__ __constant__ float d_cFilterKernel[MAX_FILTER_SIZE * MAX_FILTER_SIZE];

__global__ void backConvolution(
	float* output,
	float* input,
	float* kernel,
	int inputWidth,
	int inputHeight,
	int kernelWidth,
	int kernelHeight,
	int outWidth,
	int outHeight
)
{
	int step = 1;
	int padY = kernelHeight - step;
	int padX = kernelWidth - step;
	
	int x = threadIdx.x + blockDim.x * blockIdx.x;
	int y = threadIdx.y + blockDim.y * blockIdx.y;
	
	x -= padX;
	y -= padY;
	//int x = blockIdx.x - padX;
	//int y = blockIdx.y - padY;

	for (int h = y < 0 ? 0 - y : 0; h < (y + kernelHeight > inputHeight ? (inputHeight - (y + kernelHeight)) + kernelHeight : kernelHeight); h++)
		for (int w = x < 0 ? 0 - x : 0; w < (x + kernelWidth > inputWidth ? (inputWidth - (x + kernelWidth)) + kernelWidth : kernelWidth); w++)
			output[(y + padY) * outWidth + (x + padX)] += input[((y + h) * inputWidth + x + w)] * kernel[h * kernelWidth + w];

		/*for (int y = -(padY); y < inputWidth + padY - step; y++)
		{
			for (int x = -(padX); x < inputHeight + padX - step; x++)
			{
				for (int h = y < 0 ? 0 - y : 0; h < (y + kernelHeight > inputHeight ? (inputHeight - (y + kernelHeight)) + kernelHeight : kernelHeight); h++)
					for (int w = x < 0 ? 0 - x : 0; w < (x + kernelWidth > inputWidth ? (inputWidth - (x + kernelWidth)) + kernelWidth : kernelWidth); w++)
						output[(y + padY) * outWidth + (x + padX)] += input[((y + h) * inputWidth + x + w)] * kernel[h * kernelWidth + w];
			}
		}*/
}

__global__ void convolution(
	float* output,
	float* input,
	float* kernel,
	int inputWidth,
	int inputHeight,
	int kernelWidth,
	int kernelHeight,
	int outWidth,
	int outHeight
)
{
	int x = threadIdx.x + blockDim.x * blockIdx.x;
	int y = threadIdx.y + blockDim.y * blockIdx.y;

	if (x < inputWidth && y < inputHeight)
	{
		float sum = 0;

		for (int ky = 0; ky < kernelHeight; ky++)
		{
			for (int kx = 0; kx < kernelHeight; kx++)
			{
				output[y * outWidth + x] += (input[(y + ky) * inputWidth + (x + kx)] * kernel[ky * kernelWidth + kx]);
			}
		}
	}
}

template <typename T>
__global__ void imageFilteringKernel(const T* d_f, const unsigned int paddedW, const unsigned int paddedH, 	const int S, T* d_h, const unsigned int W, const unsigned int H)
{
	// Set the padding size and filter size
	unsigned int paddingSize = S;
	unsigned int filterSize = 2 * S + 1;

	// Set the pixel coordinate
	const unsigned int j = blockIdx.x * blockDim.x + threadIdx.x + paddingSize;
	const unsigned int i = blockIdx.y * blockDim.y + threadIdx.y + paddingSize;

	// The multiply-add operation for the pixel coordinate ( j, i )
	if (j >= paddingSize && j < paddedW - paddingSize && i >= paddingSize && i < paddedH - paddingSize) 
	{
		unsigned int oPixelPos = (i - paddingSize) * W + (j - paddingSize);
		d_h[oPixelPos] = 0.0;

		for (int k = -S; k <= S; k++) 
		{
			for (int l = -S; l <= S; l++) 
			{
				unsigned int iPixelPos = (i + k) * paddedW + (j + l);
				unsigned int coefPos = (k + S) * filterSize + (l + S);

				d_h[oPixelPos] += d_f[iPixelPos] * d_cFilterKernel[coefPos];
			}
		}
	}
}

template <typename T>
__global__ void imageFilteringKernelSh(const T* d_f, const unsigned int paddedW, const unsigned int paddedH, const unsigned int blockW, const unsigned int blockH, const int S,	T* d_h, const unsigned int W, const unsigned int H)
{

	//
	// Note that blockDim.(x,y) cannot be used instead of blockW and blockH,
	// because the size of a thread block is not equal to the size of a data block
	// due to the apron and the use of subblocks.
	//

	//
	// Set the size of a tile
	//
	const unsigned int tileW = blockW + 2 * S;
	const unsigned int tileH = blockH + 2 * S;

	// 
	// Set the number of subblocks in a tile
	//
	const unsigned int noSubBlocks = static_cast<unsigned int>(ceil(static_cast<double>(tileH) / static_cast<double>(blockDim.y)));

	//
	// Set the start position of a data block, which is determined by blockIdx. 
	// Note that since padding is applied to the input image, the origin of the block is ( S, S )
	//
	const unsigned int blockStartCol = blockIdx.x * blockW + S;
	const unsigned int blockEndCol = blockStartCol + blockW;

	const unsigned int blockStartRow = blockIdx.y * blockH + S;
	const unsigned int blockEndRow = blockStartRow + blockH;

	//
	// Set the position of the tile which includes the data block and its apron
	//
	const unsigned int tileStartCol = blockStartCol - S;
	const unsigned int tileEndCol = blockEndCol + S;
	const unsigned int tileEndClampedCol = min(tileEndCol, paddedW);

	const unsigned int tileStartRow = blockStartRow - S;
	const unsigned int tileEndRow = blockEndRow + S;
	const unsigned int tileEndClampedRow = min(tileEndRow, paddedH);

	//
	// Set the size of the filter kernel
	//
	const unsigned int kernelSize = 2 * S + 1;

	//
	// Shared memory for the tile
	//
	extern __shared__ T sData[];

	//
	// Copy the tile into shared memory
	//
	unsigned int tilePixelPosCol = threadIdx.x;
	unsigned int iPixelPosCol = tileStartCol + tilePixelPosCol;
	for (unsigned int subBlockNo = 0; subBlockNo < noSubBlocks; subBlockNo++) {

		unsigned int tilePixelPosRow = threadIdx.y + subBlockNo * blockDim.y;
		unsigned int iPixelPosRow = tileStartRow + tilePixelPosRow;

		if (iPixelPosCol < tileEndClampedCol && iPixelPosRow < tileEndClampedRow) { // Check if the pixel in the image
			unsigned int iPixelPos = iPixelPosRow * paddedW + iPixelPosCol;
			unsigned int tilePixelPos = tilePixelPosRow * tileW + tilePixelPosCol;
			sData[tilePixelPos] = d_f[iPixelPos];
		}

	}

	//
	// Wait for all the threads for data loading
	//
	__syncthreads();

	//
	// Perform convolution
	//
	tilePixelPosCol = threadIdx.x;
	iPixelPosCol = tileStartCol + tilePixelPosCol;
	for (unsigned int subBlockNo = 0; subBlockNo < noSubBlocks; subBlockNo++) {

		unsigned int tilePixelPosRow = threadIdx.y + subBlockNo * blockDim.y;
		unsigned int iPixelPosRow = tileStartRow + tilePixelPosRow;

		// Check if the pixel in the tile and image.
		// Note that the apron of the tile is excluded.
		if (iPixelPosCol >= tileStartCol + S && iPixelPosCol < tileEndClampedCol - S &&
			iPixelPosRow >= tileStartRow + S && iPixelPosRow < tileEndClampedRow - S) {

			// Compute the pixel position for the output image
			unsigned int oPixelPosCol = iPixelPosCol - S; // removing the origin
			unsigned int oPixelPosRow = iPixelPosRow - S;
			unsigned int oPixelPos = oPixelPosRow * W + oPixelPosCol;

			unsigned int tilePixelPos = tilePixelPosRow * tileW + tilePixelPosCol;

			d_h[oPixelPos] = 0.0;
			for (int i = -S; i <= S; i++) {
				for (int j = -S; j <= S; j++) {
					int tilePixelPosOffset = i * tileW + j;
					int coefPos = (i + S) * kernelSize + (j + S);
					d_h[oPixelPos] += sData[tilePixelPos + tilePixelPosOffset] * d_cFilterKernel[coefPos];
				}
			}

		}

	}

}

__global__ void sumArray(double* output, double* input, int outStride, int inStride)
{
	if (outStride > blockIdx.y && outStride > blockIdx.x)
	{
		for (int ky = 0; ky < inStride; ky++)
		{
			for (int kx = 0; kx < inStride; kx++)
			{
				output[blockIdx.y * outStride + blockIdx.x] += input[(blockIdx.y + ky) * inStride + (blockIdx.x + kx)];
			}
		}
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

		//__syncthreads();

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

/*

__global__ void ArraySum(float* array)
{
	int index = threadIdx.x;

	sum = sum + array[index];

	__syncthreads();
}*/

GridStruct getGridModel(int width, int length)
{
	GridStruct result;

	int height = length / width;
	int BSX = width > 30 ? 30 : width;
	int BSY = height > 30 ? 30 : height;

	int vx = width % BSX > 0 ? (width / BSX) + 1 : width / BSX;
	int vy = height % BSY > 0 ? (height / BSY) + 1 : height / BSY;

	result.gridDim = dim3(vx, vy, 1);
	result.threadsPerBlock = dim3(BSX, BSY, 1);

	return result;
}

extern "C" __declspec(dllexport) void Rot90GPU(double output[], double input[], int width, int length)
{
	double* dev_a, * dev_b;
	int height = length / width;

	cudaMalloc((void**)&dev_a, sizeof(double) * width * height);
	cudaMalloc((void**)&dev_b, sizeof(double) * width * height);
	cudaMemcpy(dev_a, input, sizeof(double) * width * height, cudaMemcpyHostToDevice);

	GridStruct grid = getGridModel(width, length);

	rot90 <<<grid.gridDim, grid.threadsPerBlock>>> (dev_a, dev_b, width, height);

	cudaDeviceSynchronize();

	cudaMemcpy(output, dev_b, sizeof(double) * width * height, cudaMemcpyDeviceToHost);

	cudaFree(dev_a);
	cudaFree(dev_b);
}

extern "C" __declspec(dllexport) void Rot180GPU(float output[], float input[], int width, int length)
{
	float* dev_a, * dev_b;
	int height = length / width;

	cudaMalloc((void**)&dev_a, sizeof(float) * length);
	cudaMalloc((void**)&dev_b, sizeof(float) * length);
	cudaMemcpy(dev_a, input, sizeof(float) * length, cudaMemcpyHostToDevice);

	GridStruct grid = getGridModel(width, length);
	/*int BSX = width > 30 ? 30 : width;
	int BSY = height > 30 ? 30 : height;

	dim3 gridDim = dim3(width / BSX + 1, height / BSY + 1, 1);
	dim3 threadsPerBlock = dim3(BSX, BSY, 1);*/

	rot180 <<<grid.gridDim, grid.threadsPerBlock >>> (dev_a, dev_b, width, height);

	cudaDeviceSynchronize();

	cudaMemcpy(output, dev_b, sizeof(float) * length, cudaMemcpyDeviceToHost);

	cudaFree(dev_a);
	cudaFree(dev_b);
}

extern "C" __declspec(dllexport) void MultiplyGPU(double output[], double input[], double weights[], int len)
{
	double* dev_output, * dev_a, * dev_b;

	cudaMalloc((void**)&dev_output, sizeof(double) * len);
	cudaMalloc((void**)&dev_a, sizeof(double) * len);
	cudaMalloc((void**)&dev_b, sizeof(double) * len);
	cudaMemcpy(dev_a, input, sizeof(double) * len, cudaMemcpyHostToDevice);
	cudaMemcpy(dev_b, weights, sizeof(double) * len, cudaMemcpyHostToDevice);

	const int size = 511;
	int threads = len > size ? size : len;
	int blocks = len % size > 0 ? len / threads + 1 : len / threads;

	Multiply <<<blocks, threads>>> (dev_output, dev_a, dev_b);

	cudaDeviceSynchronize();

	cudaMemcpy(output, dev_output, sizeof(double) * len, cudaMemcpyDeviceToHost);

	cudaFree(dev_output);
	cudaFree(dev_a);
	cudaFree(dev_b);
}

extern "C" __declspec(dllexport) void Multiply2GPU(float output[], float input[], float weights[], int len, int wlen, int nlen)
{
	float* dev_output, * dev_input, * dev_weights;

	cudaMalloc((void**)&dev_output, sizeof(float) * nlen);
	cudaMalloc((void**)&dev_input, sizeof(float) * len);
	cudaMalloc((void**)&dev_weights, sizeof(float) * wlen);
	cudaMemcpy(dev_input, input, sizeof(float) * len, cudaMemcpyHostToDevice);
	cudaMemcpy(dev_weights, weights, sizeof(float) * wlen, cudaMemcpyHostToDevice);

	const int size = 127;
	int threads = wlen > size ? size : wlen;
	int blocks = wlen % size > 0 ? wlen / threads + 1 : wlen / threads;

	Multiply3 <<<1, nlen>>> (dev_output, dev_input, dev_weights, len, nlen);

	cudaDeviceSynchronize();

	cudaMemcpy(output, dev_output, sizeof(float) * nlen, cudaMemcpyDeviceToHost);

	/*for (int i = 0; i < wlen; i++)
	{
		output[i / len] += *(o + i);
	}*/

	cudaFree(dev_output);
	cudaFree(dev_input);
	cudaFree(dev_weights);
}

extern "C" __declspec(dllexport) void ConvolutionGPU(
	float output[],
	float input[],
	float kernel[],
	int inputWidth,
	int inputHeight,
	int kernelWidth,
	int kernelHeight,
	int outWidth,
	int outHeight
)
{
	float* dev_input, * dev_output, * dev_kernel;

	int BSX = outWidth > 32 ? 32 : outWidth;
	int BSY = outWidth > 32 ? 32 : outWidth;

	dim3 gridDim = dim3(outWidth / BSX + 1, outWidth / BSY + 1, 1);
	dim3 threadsPerBlock = dim3(BSX, BSY, 1);

	cudaMalloc((void**)&dev_input, sizeof(float) * inputWidth * inputHeight);
	cudaMalloc((void**)&dev_output, sizeof(float) * outWidth * outHeight);
	cudaMalloc((void**)&dev_kernel, sizeof(float) * kernelWidth * kernelHeight);

	cudaMemcpy(dev_input, input, sizeof(float) * inputWidth * inputHeight, cudaMemcpyHostToDevice);
	cudaMemcpy(dev_kernel, kernel, sizeof(float) * kernelWidth * kernelHeight, cudaMemcpyHostToDevice);

	convolution<<<gridDim, threadsPerBlock >>>(dev_output, dev_input, dev_kernel, inputWidth, inputHeight, kernelWidth, kernelHeight, outWidth, outHeight);

	cudaDeviceSynchronize();

	cudaMemcpy(output, dev_output, sizeof(float) * outWidth * outHeight, cudaMemcpyDeviceToHost);

	cudaFree(dev_input);
	cudaFree(dev_output);
	cudaFree(dev_kernel);
}

//int iDivUp(int a, int b) { return ((a % b) != 0) ? (a / b + 1) : (a / b); }
inline unsigned int iDivUp(const unsigned int& a, const unsigned int& b) { return (a % b != 0) ? (a / b + 1) : (a / b); }

extern "C" __declspec(dllexport) void ConvolutionGPU2(
	float output[],
	float input[],
	float kernel[],
	int inputWidth,
	int inputHeight,
	int kernelWidth,
	int kernelHeight,
	int outWidth,
	int outHeight
)
{
	float* dev_input, * dev_output;

	unsigned int filterKernelSizeByte = kernelWidth * kernelHeight * sizeof(float);
	cudaMemcpyToSymbol(d_cFilterKernel, kernel, filterKernelSizeByte, 0, cudaMemcpyHostToDevice);

	const unsigned int S = (kernelWidth - 1) / 2;
	const dim3 grid(iDivUp(inputWidth, kernelWidth), iDivUp(inputHeight, kernelHeight));
	const dim3 threadBlock(kernelWidth, kernelHeight);

	cudaMalloc((void**)& dev_input, sizeof(float) * inputWidth * inputHeight);
	cudaMalloc((void**)& dev_output, sizeof(float) * outWidth * outHeight);
	cudaMemcpy(dev_input, input, sizeof(float) * inputWidth * inputHeight, cudaMemcpyHostToDevice);

	imageFilteringKernel <<<grid, threadBlock >>> (dev_input, inputWidth, inputHeight, S, dev_output, outWidth, outHeight);

	cudaDeviceSynchronize();

	cudaMemcpy(output, dev_output, sizeof(float) * outWidth * outHeight, cudaMemcpyDeviceToHost);

	cudaFree(dev_input);
	cudaFree(dev_output);
	cudaFree(d_cFilterKernel);
}

extern "C" __declspec(dllexport) void ConvolutionGPU3(
	float output[],
	float input[],
	float kernel[],
	int inputWidth,
	int inputHeight,
	int kernelWidth,
	int kernelHeight,
	int outWidth,
	int outHeight
)
{
	float* dev_input, * dev_output;

	unsigned int filterKernelSizeByte = kernelWidth * kernelHeight * sizeof(float);
	cudaMemcpyToSymbol(d_cFilterKernel, kernel, filterKernelSizeByte, 0, cudaMemcpyHostToDevice);

	const unsigned int S = (kernelWidth - 1) / 2;

	const unsigned int blockW = 32 - kernelWidth;
	const unsigned int blockH = 32 - kernelHeight;
	/*const unsigned int tileW = blockW + 2 * S;
	const unsigned int tileH = blockH + 2 * S;
	const dim3 grid(iDivUp(S, blockW), iDivUp(S, blockH));
	const dim3 threadBlock(tileW, tileH);*/

	const unsigned int tileW = blockW + 2 * S;
	const unsigned int tileH = blockH + 2 * S;
	const dim3 grid(iDivUp(outWidth, blockW), iDivUp(outHeight, blockH));
	const dim3 threadBlock(tileW, tileH);

	const unsigned int sharedMemorySizeByte = tileW * tileH * sizeof(float);

	cudaMalloc((void**)& dev_input, sizeof(float) * inputWidth * inputHeight);
	cudaMalloc((void**)& dev_output, sizeof(float) * outWidth * outHeight);
	cudaMemcpy(dev_input, input, sizeof(float) * inputWidth * inputHeight, cudaMemcpyHostToDevice);

	imageFilteringKernelSh << <grid, threadBlock, sharedMemorySizeByte >> > (dev_input, inputWidth, inputHeight, blockH, blockW, S, dev_output, outWidth, outHeight);

	cudaDeviceSynchronize();

	cudaMemcpy(output, dev_output, sizeof(float) * outWidth * outHeight, cudaMemcpyDeviceToHost);

	cudaFree(dev_input);
	cudaFree(dev_output);
	cudaFree(d_cFilterKernel);
}

extern "C" __declspec(dllexport) void BackConvolutionGPU(
	float output[],
	float input[],
	float kernel[],
	int inputWidth,
	int inputHeight,
	int kernelWidth,
	int kernelHeight,
	int outWidth,
	int outHeight
)
{
	float* dev_input, * dev_output, * dev_kernel;

	int step = 1;
	int padY = kernelHeight - step;
	int padX = kernelWidth - step;

	dim3 gridDim = dim3(inputWidth + padY - step, inputHeight + padX - step, 1);
	dim3 threadsPerBlock = dim3(outWidth, outHeight, 1);

	cudaMalloc((void**)& dev_input, sizeof(float) * inputWidth * inputHeight);
	cudaMalloc((void**)& dev_output, sizeof(float) * outWidth * outHeight);
	cudaMalloc((void**)& dev_kernel, sizeof(float) * kernelWidth * kernelHeight);

	cudaMemcpy(dev_input, input, sizeof(float) * inputWidth * inputHeight, cudaMemcpyHostToDevice);
	cudaMemcpy(dev_kernel, kernel, sizeof(float) * kernelWidth * kernelHeight, cudaMemcpyHostToDevice);

	backConvolution << <gridDim, threadsPerBlock >> > (dev_output, dev_input, dev_kernel, inputWidth, inputHeight, kernelWidth, kernelHeight, outWidth, outHeight);

	cudaDeviceSynchronize();

	cudaMemcpy(output, dev_output, sizeof(float) * outWidth * outHeight, cudaMemcpyDeviceToHost);

	cudaFree(dev_input);
	cudaFree(dev_output);
	cudaFree(dev_kernel);
}

extern "C" __declspec(dllexport) void BackConvolutionGPU2(
	float output[],
	float input[],
	float kernel[],
	int inputWidth,
	int inputHeight,
	int kernelWidth,
	int kernelHeight,
	int outWidth,
	int outHeight
)
{
	float* dev_input, * dev_output, * dev_kernel;

	int step = 1;
	int padY = kernelHeight - step;
	int padX = kernelWidth - step;

	/*dim3 gridDim = dim3(inputWidth + padY - step, inputHeight + padX - step, 1);
	dim3 threadsPerBlock = dim3(outWidth, outHeight, 1);*/

	int BSX = outWidth > 32 ? 32 : outWidth;
	int BSY = outWidth > 32 ? 32 : outWidth;

	dim3 gridDim = dim3(outWidth / BSX + 1, outWidth / BSY + 1, 1);
	dim3 threadsPerBlock = dim3(BSX, BSY, 1);

	cudaMalloc((void**)& dev_input, sizeof(float) * inputWidth * inputHeight);
	cudaMalloc((void**)& dev_output, sizeof(float) * outWidth * outHeight);
	cudaMalloc((void**)& dev_kernel, sizeof(float) * kernelWidth * kernelHeight);

	cudaMemcpy(dev_input, input, sizeof(float) * inputWidth * inputHeight, cudaMemcpyHostToDevice);
	cudaMemcpy(dev_kernel, kernel, sizeof(float) * kernelWidth * kernelHeight, cudaMemcpyHostToDevice);

	GridStruct grid = getGridModel(kernelWidth, kernelWidth * kernelHeight);

	rot180 << <grid.gridDim, grid.threadsPerBlock >> > (dev_kernel, dev_kernel, kernelWidth, kernelWidth * kernelHeight);
	backConvolution << <gridDim, threadsPerBlock >> > (dev_output, dev_input, dev_kernel, inputWidth, inputHeight, kernelWidth, kernelHeight, outWidth, outHeight);

	cudaDeviceSynchronize();

	cudaMemcpy(output, dev_output, sizeof(float) * outWidth * outHeight, cudaMemcpyDeviceToHost);

	cudaFree(dev_input);
	cudaFree(dev_output);
	cudaFree(dev_kernel);
}