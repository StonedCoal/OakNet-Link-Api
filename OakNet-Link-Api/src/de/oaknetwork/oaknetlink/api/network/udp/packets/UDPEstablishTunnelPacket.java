package de.oaknetwork.oaknetlink.api.network.udp.packets;

import java.net.InetAddress;
import java.net.UnknownHostException;
import java.util.HashMap;
import java.util.Map;

import de.oaknetwork.oaknetlink.api.gui.components.Dialog;
import de.oaknetwork.oaknetlink.api.log.Logger;
import de.oaknetwork.oaknetlink.api.network.udp.UDPEndpoint;
import de.oaknetwork.oaknetlink.api.network.udp.UDPEndpointHelper;

/**
 * This Packet contains information about the other peer.
 * 
 * This is required for the UDP holepunching.
 * 
 * @author Fabian Fila
 */
public class UDPEstablishTunnelPacket extends UDPPacket{

	@Override
	public Map<String, Class<?>> expectedTypes() {
		Map<String, Class<?>> expectedTypes = new HashMap<String, Class<?>>();
		expectedTypes.put("address", String.class);
		expectedTypes.put("port", Integer.class);
		
		return expectedTypes;
	}

	@Override
	protected void processPacket(Map<String, Object> data, UDPEndpoint sender) {
		Logger.logInfo("Establishing UDP tunnel...", UDPEstablishTunnelPacket.class);
		String address = (String) data.get("address");
		int port = (int) data.get("port");
		Logger.logInfo("Connecting to: " + address + ":" + port, UDPEstablishTunnelPacket.class);
		try {
			UDPEndpoint peerEndpoint = new UDPEndpoint(InetAddress.getByName(address), port);
			UDPEndpointHelper.addEndpoint(peerEndpoint);
			UDPHandshakePacket.sendPacket(peerEndpoint);
		} catch (UnknownHostException e) {
			Logger.logException("Can't connect to peer", e, UDPEstablishTunnelPacket.class);
			new Dialog(1, "Can't connect to peer", "Can't connect to peer: \n" + e.getMessage(), true, true);
			return;
		}
	}
	
	public static void sendPacket(UDPEndpoint receiver, String peerAddress, int peerPort) {
		Map<String, Object> data = new HashMap<String, Object>();
		data.put("address", peerAddress);
		data.put("port", peerPort);
		sendPacket(UDPEstablishTunnelPacket.class, receiver, data);
	}

}
