# Performance Benchmark Report: Zero-Copy Optimization

## Objective
To quantify the performance improvements gained by eliminating redundant memory copies and allocations during image transfer between C# and the OFIQ C++ library.

## Theoretical Analysis

### Before Optimization (Phase 0):
1.  **Managed Allocation:** `O(N)` where N is pixel data size.
2.  **Managed Copy:** `O(N)` (ImageSharp internal -> `byte[]`).
3.  **Native Allocation:** `O(N)` (C++ heap).
4.  **Native Copy:** `O(N)` (`byte[]` -> `uint8_t*`).
**Total:** 2 Allocations, 2 Full Memory Copies.

### After Optimization (Phase 1):
1.  **Pinning:** `O(1)` (GC overhead, but no copy).
2.  **Pointer Passing:** `O(1)`.
**Total:** 0 Allocations, 0 Memory Copies.

## Estimated Improvement
For a 1080p image (~6MB):
- **Memory saved:** ~12MB per request (managed + native buffers).
- **Latency saved:** Time taken to copy 12MB of data (approx. 5-15ms depending on hardware).
- **GC Impact:** Significantly reduced pressure on Large Object Heap (LOH).

## Benchmarking Script (for Linux)
Run the following command to measure execution time for multiple iterations:

```bash
# Compile with Release optimization
dotnet publish -c Release -r linux-x64 --self-contained false

# Run benchmark (pseudo-code, requires actual image)
time for i in {1..100}; do ./OFIQConsoleApp /path/to/image.jpg > /dev/null; done
```

## Conclusion
The zero-copy approach eliminates nearly all overhead associated with the C#/C++ boundary, making the assessment process bound strictly by the OFIQ library's internal processing (mostly ONNX model inference time).
