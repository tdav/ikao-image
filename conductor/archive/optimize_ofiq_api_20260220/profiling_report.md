# Profiling Report: Image Transfer (ImageSharp to C++ Bridge)

## Current Implementation Analysis

The current process of passing image data from C# to the OFIQ C++ library involves multiple allocations and copies:

1.  **Managed Allocation:** `new byte[width * height * 3]` creates a large array on the managed heap.
2.  **Managed Copy:** `image.CopyPixelDataTo(pixelData)` copies pixels from ImageSharp's internal memory to the managed array.
3.  **P/Invoke Marshalling:** The array is passed to C++. While .NET can pin arrays, it's still a transition.
4.  **Native Allocation:** `new uint8_t[size]` in `ofiq_bridge.cpp` allocates native memory.
5.  **Native Copy:** `memcpy(data_copy, data, size)` copies data from the pinned managed array to the native buffer.
6.  **Shared Pointer Wrapping:** The native buffer is wrapped in a `std::shared_ptr`.

## Bottlenecks Identified

1.  **Redundant Copies:** Data is copied twice before reaching the OFIQ library.
2.  **Memory Pressure:** Frequent allocation of large pixel buffers (e.g., ~6MB for a 1080p image) triggers frequent GC collections in C# and heap fragmentation in C++.
3.  **Latency:** For high-resolution images, the copy operations add measurable latency to each assessment.

## Proposed Optimizations

1.  **Direct Pointer Access:** Use `image.DangerousGetPixelRowMemory(0)` or similar to get a pointer to the raw ImageSharp buffer and pass it directly to P/Invoke.
2.  **Avoid Native Copy:** Update the C++ bridge to either use the passed pointer directly (if OFIQ allows) or use a custom deleter for `std::shared_ptr` that doesn't own the memory (risk: memory must remain pinned).
3.  **Buffer Pooling:** Use `ArrayPool<byte>.Shared` in C# to reuse pixel buffers.
