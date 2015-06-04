#include "pch.h"
#include "ScreenProperties.h"
#include <agile.h>
#include <windows.ui.xaml.media.dxinterop.h>

using namespace Microsoft::WRL;
using namespace Windows::Foundation;
using namespace RdClientCxHelpers;
using namespace Platform;

ScreenProperties::ScreenProperties()
{
	// This flag adds support for surfaces with a different color channel ordering than the API default.
	// It is recommended usage, and is required for compatibility with Direct2D.
	UINT creationFlags = D3D11_CREATE_DEVICE_BGRA_SUPPORT;

	// This array defines the set of DirectX hardware feature levels this app will support.
	// Note the ordering should be preserved.
	// Don't forget to declare your application's minimum required feature level in its
	// description.  All applications are assumed to support 9.1 unless otherwise stated.
	D3D_FEATURE_LEVEL featureLevels[] =
	{
		D3D_FEATURE_LEVEL_11_1,
		D3D_FEATURE_LEVEL_11_0,
		D3D_FEATURE_LEVEL_10_1,
		D3D_FEATURE_LEVEL_10_0,
		D3D_FEATURE_LEVEL_9_3,
		D3D_FEATURE_LEVEL_9_2,
		D3D_FEATURE_LEVEL_9_1
	};

	// Create the DX11 API device object, and get a corresponding context.
	ComPtr<ID3D11DeviceContext> context;
	ThrowIfFailed(
		D3D11CreateDevice(
			nullptr,                    // specify null to use the default adapter
			D3D_DRIVER_TYPE_HARDWARE,
			0,                          // leave as 0 unless software device
			creationFlags,              // optionally set debug and Direct2D compatibility flags
			featureLevels,              // list of feature levels this app can support
			ARRAYSIZE(featureLevels),   // number of entries in above list
			D3D11_SDK_VERSION,          // always set this to D3D11_SDK_VERSION for Metro style apps
			&m_d3dDevice,                    // returns the Direct3D device created
			&m_featureLevel,            // returns feature level of device created
			&context                    // returns the device immediate context
			)
		);

	ThrowIfFailed(
		D2D1CreateFactory(D2D1_FACTORY_TYPE_SINGLE_THREADED,
			__uuidof(ID2D1Factory), (void**)&m_d2dFactory)
		);
}	

Windows::Foundation::Size ScreenProperties::Resolution::get()
{
	ComPtr<IDXGIDevice> dxgiDevice;
	ThrowIfFailed(
		m_d3dDevice.As(&dxgiDevice)
		);

	ComPtr<IDXGIAdapter> dxgiAdapter;
	ThrowIfFailed(
		dxgiDevice->GetAdapter(&dxgiAdapter)
		);

	IDXGIOutput * pOutput;
	if (dxgiAdapter->EnumOutputs(0, &pOutput) != DXGI_ERROR_NOT_FOUND)
	{
		Windows::Foundation::Size size;
		DXGI_OUTPUT_DESC desc;
		ThrowIfFailed(
			pOutput->GetDesc(&desc)
			);
		pOutput->Release();

		FLOAT dpiX, dpiY;
		m_d2dFactory->GetDesktopDpi(&dpiX, &dpiY);

		size.Width = (float)desc.DesktopCoordinates.right * 96.0f / dpiX;
		size.Height = (float)desc.DesktopCoordinates.bottom * 96.0f / dpiY;

		return size;
	}

	return Windows::Foundation::Size(1024, 768);
}