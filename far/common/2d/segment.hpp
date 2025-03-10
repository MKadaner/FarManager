#ifndef SEGMENT_HPP_DC38F7F7_0E1B_486E_8368_2ACE6415D234
#define SEGMENT_HPP_DC38F7F7_0E1B_486E_8368_2ACE6415D234
#pragma once

/*
segment.hpp

*/
/*
Copyright © 2018 Far Group
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:
1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. The name of the authors may not be used to endorse or promote products
   derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

//----------------------------------------------------------------------------

template<typename T>
struct segment_width_t
{
	T width;

	explicit operator T() const noexcept { return width; }
};

template<typename T>
struct segment_t
{
	T begin;
	T end; // One past last

	segment_t() noexcept = default;

	segment_t(T const Begin, T const End) noexcept
		: begin{ Begin }
		, end{ End }
	{
		//assert(begin <= end); // Segment may be empty
	}

	segment_t(T const Begin, segment_width_t<T> const Width) noexcept
		: segment_t{ Begin, static_cast<T>(Width) }
	{
	}

	template<typename Y>
	explicit(false) segment_t(segment_t<Y> const Segment) noexcept:
		segment_t(Segment.begin, Segment.end)
	{
	}

	bool operator==(segment_t const&) const = default;

	[[nodiscard]]
	auto width() const noexcept { assert(begin <= end); return end - begin; }

	[[nodiscard]]
	auto empty() const noexcept { return !width(); }

	[[nodiscard]]
	auto first() const noexcept { return begin; }

	[[nodiscard]]
	auto last() const noexcept { return end - 1; }
};

using small_segment_width = segment_width_t<short>;
using segment_width = segment_width_t<int>;

using small_segment = segment_t<short>;
using segment = segment_t<int>;

#endif // SEGMENT_HPP_DC38F7F7_0E1B_486E_8368_2ACE6415D234
