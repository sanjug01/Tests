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
		Microsoft::WRL::ComPtr<ID3D11Device>           m_d3dDevice;
		Microsoft::WRL::ComPtr<ID2D1Factory>            m_d2dFactory;
	};
}
