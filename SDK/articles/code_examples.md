`C#` EXAMPLE FOR RIS
====================

Here is a **minimal** `C#` example for making the RIS call using our current `.NET SDK`.

Step-by-step guide
-------------------
### Add the steps involved:

1. **Add** the `Kount.NET` Library to your Project.
2. **Create** a `Kount.Ris.Inquiry` Object and populate the setters.
3. **Add** `cart` data.
4. **Ask** for the responger(`Inquiry.getResponse()`).
5. **Process** the Kount.Ris.Response object returned.

```cs
Kount.Ris.Inquiry inq = new Kount.Ris.Inquiry();
inq.SetTotal(5000);
inq.SetPayment("CARD", "5789372819873789");
inq.SetIpAddress("165.53.125.33");
inq.SetMack('Y');
inq.SetEmail("joe@domain.com");
inq.SetMode('Q');
inq.SetSessionId("vdt8796tbhbvhe786hret87645643");
inq.SetWebsite("DEFAULT");
ArrayList cart = new ArrayList();
cart.Add(new Kount.Ris.CartItem("Electronics", "TV","Big TV", 1, 24900));
inq.SetCart (cart);
Kount.Ris.Response response = inq.GetResponse();
Console.WriteLine("RESPONSE: " + response.ToString());
```