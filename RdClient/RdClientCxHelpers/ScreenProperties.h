#pragma once

namespace RdClientCxHelpers
{
	inline void ThrowIfFailed(HRESULT hr)
	{
		if (FAILED(hr))
		{
			// Set a breakpoint on this line to catch DirectX API errors
			throw Platform::Exception::CreateException(hr);
		}
	}

	public ref class ScreenProperties sealed
	{
	public:
		ScreenProperties();
		property Windows::Foundation::Size Resolution
		{
			Windows::Foundation::Size get();
		}

	private:
		D3D_FEATURE_LEVEL							    m_featureLevel;
		Microsoft::WRL::ComPtr<ID3D11Device1>           m_d3dDevice;
	};
}
